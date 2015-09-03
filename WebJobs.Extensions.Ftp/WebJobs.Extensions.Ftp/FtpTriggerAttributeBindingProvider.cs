using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ArxOne.Ftp;
using Microsoft.Azure.WebJobs.Extensions.Files;
using Microsoft.Azure.WebJobs.Extensions.Framework;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using WebJobs.Extensions.Ftp;
using WebJobs.Extensions.Ftp.Listener;

namespace Webjobs.Extensions.Ftp
{
    public class FtpTriggerValue
    {
        public string FileContent { get; set; }
        public string Filename { get; set; }

        string CreatedDate { get; set; }
        // TODO: Define the default type that your trigger binding
        // binds to (the type representing the trigger event).
    }

    internal class FtpTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        private readonly FtpConfiguration _config;
        private readonly TraceWriter _trace;

        public FtpTriggerAttributeBindingProvider(FtpConfiguration config, TraceWriter trace)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            if (trace == null)
            {
                throw new ArgumentNullException("trace");
            }

            _config = config;
            _trace = trace;
        }

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

            IEnumerable<Type> types = StreamValueBinder.SupportedTypes.Union(new Type[] { typeof(FileStream), typeof(FileSystemEventArgs), typeof(FileInfo) });
            if (!ValueBinder.MatchParameterType(context.Parameter, types))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Can't bind FtpTriggerAttribute to type '{0}'.", parameter.ParameterType));
            }


            return Task.FromResult<ITriggerBinding>(new FtpTriggerBinding(_config, parameter, _trace));
        }

        private class FtpTriggerBinding : ITriggerBinding
        {
            private readonly ParameterInfo _parameter;
            private readonly FileTriggerAttribute _attribute;
            private readonly FtpConfiguration _config;
            private readonly BindingContract _bindingContract;
            private readonly TraceWriter _trace;

            public FtpTriggerBinding(FtpConfiguration config, ParameterInfo parameter, TraceWriter trace)
            {
                _config = config;
                _parameter = parameter;
                _trace = trace;
                _attribute = parameter.GetCustomAttribute<FileTriggerAttribute>(inherit: false);
                //_bindingContract = CreateBindingContract();
            }

            public IReadOnlyDictionary<string, Type> BindingDataContract
            {
                get
                {
                    return _bindingContract.BindingDataContract;
                }
            }

            //TODO: change this
            public Type TriggerValueType
            {
                get { return typeof(FileSystemEventArgs); }
            }

            public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
            {
                FtpTriggerValue ftv = new FtpTriggerValue();
                
                FileSystemEventArgs fileEvent = value as FileSystemEventArgs;
                if (fileEvent == null)
                {
                    string filePath = value as string;
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        // TODO: This only supports Created events. For Dashboard invocation, how can we
                        // handle Change events?
                        string directory = Path.GetDirectoryName(filePath);
                        string fileName = Path.GetFileName(filePath);

                        fileEvent = new FileSystemEventArgs(WatcherChangeTypes.Created, directory, fileName);
                    }
                }

                IValueBinder valueBinder = new FtpValueBinder(_parameter, ftv);
                IReadOnlyDictionary<string, object> bindingData = GetBindingData(ftv);

                return Task.FromResult<ITriggerData>(new TriggerData(valueBinder, bindingData));
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
                    NetworkCredential credential;
                    var connectionString = ConfigurationManager.ConnectionStrings["AzureWebJobsFtp"].ConnectionString;
                    Uri uri = CreateFtpUriFromConnectionString(connectionString, out credential);


                    using (var ftpClient = new FtpClient(uri, credential))
                    {
                        var lista = ftpClient.ListEntries("hej");
                        foreach (FtpEntry ftpEntry in lista)
                        {
                            Stream s = ftpClient.Retr(ftpEntry.Path, FtpTransferMode.Binary);

                            StreamReader reader = new StreamReader(s);
                            string text = reader.ReadToEnd();

                            TriggeredFunctionData input = new TriggeredFunctionData
                            {
                                TriggerValue = new FtpTriggerValue
                                {
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

                private Uri CreateFtpUriFromConnectionString(string connectionString, out NetworkCredential credentials)
                {
                    // TODO: extract user/pass from connection string
                    // TODO: Validate connection string
                    credentials = new NetworkCredential("mats","mats");
                    return new Uri("ftp://localhost");
                }
             
            }
        }
    }
}
