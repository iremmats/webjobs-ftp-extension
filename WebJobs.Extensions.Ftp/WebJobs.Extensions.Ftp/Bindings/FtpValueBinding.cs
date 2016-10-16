using Microsoft.Azure.WebJobs.Extensions.Framework;
using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebJobs.Extensions.Ftp.Client;
using WebJobs.Extensions.Ftp.Model;

namespace WebJobs.Extensions.Ftp.Bindings
{
    public class FtpValueBinder : IValueBinder

    {
        private readonly IFtpClient _client;

        public FtpValueBinder(IFtpClient client)
        {
            _client = client;
        }

        public object GetValue() => null;

        public string ToInvokeString() => String.Empty;

        public Type Type { get; }

        public async Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            var file = value as FtpFile;
            await _client.SendFileAsync(file.Filename, file.Stream);
        }
    }
}