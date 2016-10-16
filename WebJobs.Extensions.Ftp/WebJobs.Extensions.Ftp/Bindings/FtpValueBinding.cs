using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebJobs.Extensions.Ftp.Client;
using WebJobs.Extensions.Ftp.Model;

namespace WebJobs.Extensions.Ftp.Bindings
{
    public class FtpValueBinder : IValueBinder

    {
        private readonly IFtpClient _client;
        private readonly FtpAttribute _ftpAttribute;

        public FtpValueBinder(IFtpClient client, FtpAttribute ftpAttribute)
        {
            _client = client;
            _ftpAttribute = ftpAttribute;
        }

        public object GetValue()
        {
            if (_ftpAttribute.ReadOnStartup) Console.WriteLine("Hej");
            else Console.WriteLine("Då");
            return null;
        }

        public string ToInvokeString() => String.Empty;

        public Type Type { get; }

        public async Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            var file = value as FtpFile;
            await _client.SendFileAsync(file.Filename, file.Stream);
        }
    }
}