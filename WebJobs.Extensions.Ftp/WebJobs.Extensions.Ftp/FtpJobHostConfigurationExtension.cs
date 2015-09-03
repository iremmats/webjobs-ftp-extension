using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using Webjobs.Extensions.Ftp;

namespace WebJobs.Extensions.Ftp
{
    public static class FtpJobHostConfigurationExtensions
    {
        public static void UseFtp(this JobHostConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            FtpConfiguration ftpConfiguration = new FtpConfiguration(TimeSpan.FromMinutes(1));
            config.UseFtp(ftpConfiguration);
        }

        public static void UseFtp(this JobHostConfiguration config, FtpConfiguration ftpConfiguration)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            // Register our extension configuration provider
            config.RegisterExtensionConfigProvider(new FtpExtensionConfig(ftpConfiguration));
        }

        private class FtpExtensionConfig : IExtensionConfigProvider
        {
            private FtpConfiguration _ftpConfiguration;

            public FtpExtensionConfig(FtpConfiguration ftpConfiguration)
            {
                _ftpConfiguration = ftpConfiguration;
            }
            public void Initialize(ExtensionConfigContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }

                // Register our extension binding providers
                context.Config.RegisterBindingExtensions(
                    new FtpTriggerAttributeBindingProvider(_ftpConfiguration, context.Trace)
                );

           }
        }
    }
}
