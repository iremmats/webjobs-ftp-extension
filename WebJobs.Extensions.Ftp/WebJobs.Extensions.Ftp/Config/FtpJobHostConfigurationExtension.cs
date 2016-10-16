using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using System;
using WebJobs.Extensions.Ftp.Bindings;

namespace WebJobs.Extensions.Ftp.Config
{
    public static class FtpJobHostConfigurationExtensions
    {
        public static void UseFtp(this JobHostConfiguration config, FtpConfiguration ftpConfiguration)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            // Register our extension configuration provider
            config.RegisterExtensionConfigProvider(new FtpExtensionConfig(ftpConfiguration));
        }

        public static void UseFtp(this JobHostConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            // Register our extension configuration provider
            config.RegisterExtensionConfigProvider(new FtpExtensionConfig(new FtpConfiguration()));
        }

        private class FtpExtensionConfig : IExtensionConfigProvider
        {
            private readonly FtpConfiguration _ftpConfiguration;

            public FtpExtensionConfig(FtpConfiguration ftpConfiguration)
            {
                _ftpConfiguration = ftpConfiguration;
            }

            public void Initialize(ExtensionConfigContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                // Register our extension binding providers
                context.Config.RegisterBindingExtensions(
                    new FtpAttributeBindingProvider(_ftpConfiguration, context.Trace)
                    //new FtpTriggerAttributeBindingProvider(_ftpConfiguration, context.Trace)
                );
            }
        }
    }
}