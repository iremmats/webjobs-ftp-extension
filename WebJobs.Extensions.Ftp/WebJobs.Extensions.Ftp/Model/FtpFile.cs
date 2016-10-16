using System.IO;

namespace WebJobs.Extensions.Ftp.Model
{
    public class FtpFile
    {
        public string Filename { get; set; }
        public Stream Stream { get; set; }
    }
}