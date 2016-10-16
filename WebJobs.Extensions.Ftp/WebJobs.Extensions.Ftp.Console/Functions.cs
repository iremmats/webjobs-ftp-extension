using System;
using System.IO;
using System.Text;
using Renci.SshNet;
using WebJobs.Extensions.Ftp.Model;

namespace WebJobs.Extensions.Ftp.Console
{
    public class Functions
    {
        public static void SendFileToFtps([Ftp] out FtpMessage ftpMessage)
        {

            var stream = new MemoryStream(Encoding.UTF8.GetBytes("Nu skickar vi en fil!"));

            ftpMessage = new FtpMessage
            {
                Filename = "/tmp/testfile.txt",
                Data = stream,
                FtpHost = new Uri("sftp://sftp.com"),
                FtpPort = 22,
                Username = "",
                Password = ""

            };
        }
    }
}