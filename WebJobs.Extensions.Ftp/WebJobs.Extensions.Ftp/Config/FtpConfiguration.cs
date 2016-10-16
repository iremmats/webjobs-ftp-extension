using System;

namespace WebJobs.Extensions.Ftp.Config
{
    public class FtpConfiguration
    {
        public FtpConfiguration(
            string username,
            string password,
            Uri host,
            int port = 22
            )
        {
            Username = username;
            Password = password;
            Host = host;
            Port = port;
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public Uri Host { get; set; }
        public int Port { get; set; }
        public TimeSpan PollingInterval { get; set; }
    }
}