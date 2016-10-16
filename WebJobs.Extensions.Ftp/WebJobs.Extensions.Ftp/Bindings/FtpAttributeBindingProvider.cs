using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.Bindings;
using WebJobs.Extensions.Ftp.Config;
using WebJobs.Extensions.Ftp.Model;

namespace WebJobs.Extensions.Ftp.Bindings
{
    public sealed class FtpAttributeBindingProvider : IBindingProvider

    {
        private readonly FtpConfiguration _config;
        private TraceWriter _trace;

        public FtpAttributeBindingProvider(FtpConfiguration config, TraceWriter trace)
        {
            _config = config;
            _trace = trace;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            var parameter = context.Parameter;
            var attribute = parameter.GetCustomAttributes(typeof(FtpAttribute), inherit: false);
            if (attribute == null)
                return Task.FromResult<IBinding>(null);

            // TODO: Include any other parameter types this binding supports in this check
            var supportedTypes = new List<Type> { typeof(FtpFile) };
            if (!ValueBinder.MatchParameterType(context.Parameter, supportedTypes))
                throw new InvalidOperationException($"Can't bind {nameof(FtpAttribute)} to type '{parameter.ParameterType}'.");

            return Task.FromResult<IBinding>(new FtpBinding(_config, parameter, _trace, (FtpAttribute)attribute[0]));
        }
    }
}