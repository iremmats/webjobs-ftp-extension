using System;
using WebJobs.Extensions.Ftp.Config;

namespace WebJobs.Extensions.Ftp
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FtpAttribute : Attribute
    {
    }
}