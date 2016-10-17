using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
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
        private readonly FtpAttribute _ftpAttribute;

        public FtpBinding(FtpConfiguration config, ParameterInfo parameter, TraceWriter trace, FtpAttribute ftpAttribute)
        {
            _config = config;
            _parameter = parameter;
            _trace = trace;
            _ftpAttribute = ftpAttribute;
        }

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context) =>
            Task.FromResult<IValueProvider>(new FtpValueBinder(new FtpClient(_config),_config, _ftpAttribute));

        public Task<IValueProvider> BindAsync(BindingContext context) =>
            Task.FromResult<IValueProvider>(new FtpValueBinder(new FtpClient(_config), _config, _ftpAttribute));

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