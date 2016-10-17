using System;
using WebJobs.Extensions.Ftp.Config;

namespace WebJobs.Extensions.Ftp
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FtpAttribute : Attribute
    {
        public FtpAttribute()
        {
            
        }

        public string Path;
        public bool ReadOnStartup;

        //public FtpAttribute(Uri host,int port  ,string username ,string password )
        //{
        //    Configuration = new FtpConfiguration(host, port, username, password);
        //}
        //public FtpConfiguration Configuration { get; set; }
    }
}