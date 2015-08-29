using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using ArxOne.Ftp;
using Microsoft.Azure.WebJobs.Extensions.Framework;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using WebJobs.Extensions.Ftp;

namespace Webjobs.Extensions.Ftp
{
    public class FtpTriggerValue
    {
        public string FileContent { get; set; }
        public string Filename { get; set; }

        public string CreatedDate { get; set; }
        // TODO: Define the default type that your trigger binding
        // binds to (the type representing the trigger event).
    }

    internal class FtpTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ParameterInfo parameter = context.Parameter;
            FtpTriggerAttribute attribute = parameter.GetCustomAttribute<FtpTriggerAttribute>(inherit: false);
            if (attribute == null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }

            // TODO: Define the types your binding supports here
            if (parameter.ParameterType != typeof(FtpTriggerValue) &&
                parameter.ParameterType != typeof(string))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Can't bind SampleTriggerAttribute to type '{0}'.", parameter.ParameterType));
            }

            return Task.FromResult<ITriggerBinding>(new FtpTriggerBinding(context.Parameter, new FtpConfiguration(attribute.Server, attribute.Path, attribute.Username, attribute.Password, attribute.Filemask)));
        }

        private class FtpTriggerBinding : ITriggerBinding
        {
            private readonly ParameterInfo _parameter;
            private readonly IReadOnlyDictionary<string, Type> _bindingContract;
            private readonly FtpConfiguration _config;

            public FtpTriggerBinding(ParameterInfo parameter, FtpConfiguration config)
            {
                _parameter = parameter;
                _config = config;
                _bindingContract = CreateBindingDataContract();
            }

            public IReadOnlyDictionary<string, Type> BindingDataContract
            {
                get { return _bindingContract; }
            }

            public Type TriggerValueType
            {
                get { return typeof(FtpTriggerValue); }
            }

            public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
            {
                // TODO: Perform any required conversions on the value
                // E.g. convert from Dashboard invoke string to our trigger
                // value type
                FtpTriggerValue triggerValue = value as FtpTriggerValue;
                IValueBinder valueBinder = new FtpValueBinder(_parameter, triggerValue);
                return Task.FromResult<ITriggerData>(new TriggerData(valueBinder, GetBindingData(triggerValue)));
            }

            public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
            {
                return Task.FromResult<IListener>(new Listener(context.Executor, _config));
            }

            public ParameterDescriptor ToParameterDescriptor()
            {
                return new FtpTriggerParameterDescriptor
                {
                    Name = _parameter.Name,
                    DisplayHints = new ParameterDisplayHints
                    {
                        // TODO: Customize your Dashboard display strings
                        Prompt = "Sample",
                        Description = "Sample trigger fired",
                        DefaultValue = "Sample"
                    }
                };
            }

            private IReadOnlyDictionary<string, object> GetBindingData(FtpTriggerValue value)
            {
                Dictionary<string, object> bindingData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                bindingData.Add("FtpTrigger", value);

                // TODO: Add any additional binding data

                return bindingData;
            }

            private IReadOnlyDictionary<string, Type> CreateBindingDataContract()
            {
                Dictionary<string, Type> contract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
                contract.Add("FtpTrigger", typeof(FtpTriggerValue));

                // TODO: Add any additional binding contract members

                return contract;
            }

            private class FtpTriggerParameterDescriptor : TriggerParameterDescriptor
            {
                public override string GetTriggerReason(IDictionary<string, string> arguments)
                {
                    // TODO: Customize your Dashboard display string
                    return string.Format("My trigger fired at {0}", DateTime.UtcNow.ToString("o"));
                }
            }

            private class FtpValueBinder : ValueBinder
            {
                private readonly object _value;

                public FtpValueBinder(ParameterInfo parameter, FtpTriggerValue value)
                    : base(parameter.ParameterType)
                {
                    _value = value;
                }

                public override object GetValue()
                {
                    // TODO: Perform any required conversions
                    if (Type == typeof(string))
                    {
                        return _value.ToString();
                    }
                    return _value;
                }

                public override string ToInvokeString()
                {
                    // TODO: Customize your Dashboard invoke string
                    return "Sample";
                }
            }

            private class Listener : IListener
            {
                private ITriggeredFunctionExecutor _executor;
                private System.Timers.Timer _timer;
                private FtpConfiguration _config;

                public Listener(ITriggeredFunctionExecutor executor, FtpConfiguration config)
                {
                    _executor = executor;
                    _config = config;

                    // TODO: For this sample, we're using a timer to generate
                    // trigger events. You'll replace this with your event source.
                    _timer = new System.Timers.Timer(5 * 1000)
                    {
                        AutoReset = true
                    };


                    _timer.Elapsed += ConnectToFtpSite;
                    //_timer.Elapsed += OnTimer;
                }

                public Task StartAsync(CancellationToken cancellationToken)
                {
                    // TODO: Start monitoring your event source
                    _timer.Start();
                    return Task.FromResult(true);
                }

                public Task StopAsync(CancellationToken cancellationToken)
                {
                    // TODO: Stop monitoring your event source
                    _timer.Stop();
                    return Task.FromResult(true);
                }

                public void Dispose()
                {
                    // TODO: Perform any final cleanup
                    _timer.Dispose();
                }

                public void Cancel()
                {
                    // TODO: cancel any outstanding tasks initiated by this listener
                }    

                private void ConnectToFtpSite(object sender, System.Timers.ElapsedEventArgs e)
                {

                    var credential = new NetworkCredential(_config.Username, _config.Password);

                    using (var ftpClient = new FtpClient(new Uri("ftp://" + _config.Server), credential))
                    {
                        var lista = ftpClient.ListEntries(_config.Path);
                        foreach (FtpEntry ftpEntry in lista)
                        {
                            Stream s = ftpClient.Retr(ftpEntry.Path, FtpTransferMode.Binary);

                            StreamReader reader = new StreamReader(s);
                            string text = reader.ReadToEnd();

                            TriggeredFunctionData input = new TriggeredFunctionData
                            {
                                TriggerValue = new FtpTriggerValue
                                {
                                    CreatedDate = ftpEntry.Date.ToString(),
                                    FileContent = text,
                                    Filename = ftpEntry.Name
                                }
                            };
                            _executor.TryExecuteAsync(input, CancellationToken.None).Wait();
                            Console.WriteLine("Function finished. Now deleting file");
                            ftpClient.Delete(ftpEntry.Path);
                            Console.WriteLine(ftpEntry.Path + " has been deleted.");
                        }
                    }
                }
             
            }
        }
    }
}
