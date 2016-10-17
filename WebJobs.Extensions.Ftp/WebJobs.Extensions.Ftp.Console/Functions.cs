using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using System.IO;
using System.Text;
using WebJobs.Extensions.Ftp.Model;

namespace WebJobs.Extensions.Ftp.Console
{
    public class Functions
    {
        public static void SendFileToFtps([Ftp]  out FtpMessage ftpMessage)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("Nu skickar vi en fil!"));
            ftpMessage = new FtpMessage
            {
                Filename = "/tmp/testfile.txt",
                Data = stream
            };
        }

        public static void StartONServiceBus([ServiceBusTrigger("")] BrokeredMessage messagein,
                [Ftp("filnamn.txt", cog)] FtpMessage ftpMessage, [Ftp(config2)] FtpMessage ftpMessage2) =>
            ftpMessage = ftpMessage2;
    }
}