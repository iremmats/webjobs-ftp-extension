using System;

namespace WebJobs.Extensions.Ftp
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FtpTriggerAttribute : Attribute
    {
        public string Path { get; set; }
        public string Filemask { get; set; }
    }
}