using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJobs.Extensions.Ftp
{
    public class FtpConfiguration
    {
        public FtpConfiguration(string server, string path, string username, string password, string filemask)
        {
            Server = server;
            Path = path;
            Username = username;
            Password = password;
            Filemask = filemask;
        }
        public string Server { get; private set; }
        public string Path { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public string Filemask { get; private set; }

    }
}
