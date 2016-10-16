using System;
using System.IO;
using System.Text;
using WebJobs.Extensions.Ftp.Model;

namespace WebJobs.Extensions.Ftp.Console
{
    public class Functions
    {
        public static void SendFileToFtps([Ftp(ReadOnStartup = true)] out FtpMessage file)
        {

            var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hej"));

            file = new FtpMessage
            {
                Filename = "",
                Data = stream,
                FtpHost = new Uri(""),
                FtpPort = 21,
                Password = "",
                Username = ""
                
            };
        }
    }
}