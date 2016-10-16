using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using WebJobs.Extensions.Ftp.Client;
using WebJobs.Extensions.Ftp.Config;

namespace WebJobs.Extensions.Ftp.Bindings
{
    public sealed class FtpBinding : IBinding
    {
        private readonly ParameterInfo _parameter;
        private readonly FtpConfiguration _config;
        private TraceWriter _trace;

        public FtpBinding(FtpConfiguration config, ParameterInfo parameter, TraceWriter trace)
        {
            _config = config;
            _parameter = parameter;
            _trace = trace;
        }

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context) =>
            Task.FromResult<IValueProvider>(new FtpValueBinder(new FtpClient(_config)));

        public Task<IValueProvider> BindAsync(BindingContext context) =>
            Task.FromResult<IValueProvider>(new FtpValueBinder(new FtpClient(_config)));

        public ParameterDescriptor ToParameterDescriptor() =>
             new ParameterDescriptor
             {
                 Name = _parameter.Name,
                 DisplayHints = new ParameterDisplayHints
                 {
                     // TODO: Define your Dashboard integration strings here.
                     Description = "Sample",
                     DefaultValue = "Sample",
                     Prompt = "Please enter a Sample value"
                 }
             };

        public bool FromAttribute => true;
    }
}