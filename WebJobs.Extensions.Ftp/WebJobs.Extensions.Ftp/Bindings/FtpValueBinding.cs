using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebJobs.Extensions.Ftp.Client;
using WebJobs.Extensions.Ftp.Config;
using WebJobs.Extensions.Ftp.Model;

namespace WebJobs.Extensions.Ftp.Bindings
{
    public class FtpValueBinder : IValueBinder

    {
        private readonly IFtpClient _client;
        private readonly FtpConfiguration _config;
        private readonly FtpAttribute _ftpAttribute;

        public FtpValueBinder(IFtpClient client, FtpConfiguration config, FtpAttribute ftpAttribute)
        {
            _client = client;
            _config = config;
            _ftpAttribute = ftpAttribute;
        }

        public object GetValue()
        {
            if (_ftpAttribute.ReadOnStartup)
            {
                var task = _client.GetFileAsync(_config, _ftpAttribute.Path);
                task.Wait();
                return task.Result;
            }
            //TODO: Allow access to a file on ftp and not just send to a file
            return null;
        }

        public string ToInvokeString() => String.Empty;

        public Type Type { get; }

        public async Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            var ftpMessage = value as FtpMessage;
            await _client.SendFileAsync(ftpMessage);
        }
    }
}