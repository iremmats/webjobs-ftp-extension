using System;

namespace WebJobs.Extensions.Ftp
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FtpAttribute : Attribute
    {
        public bool ReadOnStartup;
    }
}