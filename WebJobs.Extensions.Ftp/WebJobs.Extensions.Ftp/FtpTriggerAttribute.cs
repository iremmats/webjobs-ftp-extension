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
        public string Filemask { get; set; }

    }
}
