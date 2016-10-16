using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJobs.Extensions.Ftp.Model
{
    public class FtpMessage
    {
        public string Filename { get; set; }
        public Uri FtpHost { get; set; }
        public int FtpPort { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Stream Data { get; set; }

    }
}
