using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebJobs.Extensions.Ftp.Config;

namespace WebJobs.Extensions.Ftp.Model
{
    public class FtpMessage
    {
        public string Filename { get; set; }
        public FtpConfiguration Configuration { get; set; }
        public Stream Data { get; set; }
        public bool Send { get; set; }
    }
}