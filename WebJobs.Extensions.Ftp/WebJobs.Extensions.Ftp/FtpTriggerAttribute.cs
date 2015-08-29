using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJobs.Extensions.Ftp
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FtpTriggerAttribute : Attribute
    {

        public string Path { get; set; }
        public string Server { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string Filemask { get; set; }

    }
}
