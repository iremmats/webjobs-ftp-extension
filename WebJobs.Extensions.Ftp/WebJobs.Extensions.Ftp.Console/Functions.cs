using System.IO;
using System.Text;
using WebJobs.Extensions.Ftp.Model;

namespace WebJobs.Extensions.Ftp.Console
{
    public class Functions
    {
        public static void myAction([Ftp] out FtpFile file)
        {
            file = new FtpFile
            {
                Filename = "/teamhakantest/ftpTest.txt",
                Stream = new MemoryStream(Encoding.UTF8.GetBytes("Hej"))
            };
        }
    }
}