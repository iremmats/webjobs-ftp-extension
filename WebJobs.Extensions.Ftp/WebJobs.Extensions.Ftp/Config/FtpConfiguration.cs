using System;

namespace WebJobs.Extensions.Ftp.Config
{
    public class FtpConfiguration
    {
        public FtpConfiguration(Uri ftpHost, int ftpPort, string username, string password)
        {
            FtpHost = ftpHost;
            FtpPort = ftpPort;
            Username = username;
            Password = password;
        }

        //TODO: implement this
        //public string RootFolder { get; set; }
        public Uri FtpHost { get; set; }

        public int FtpPort { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}