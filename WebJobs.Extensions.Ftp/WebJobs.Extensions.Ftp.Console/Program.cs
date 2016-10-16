using Microsoft.Azure.WebJobs;
using System;
using WebJobs.Extensions.Ftp.Config;

namespace WebJobs.Extensions.Ftp.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var jobhostConfiguration = new JobHostConfiguration
            {
                StorageConnectionString =
                    "",
                DashboardConnectionString =
                    ""
            };
            jobhostConfiguration.UseFtp(new FtpConfiguration(
                username: "",
                password: "",
                host: new Uri("")
                ));
            using (var host = new JobHost(jobhostConfiguration))
            {
                host.Call(typeof(Functions).GetMethod("myAction"));
            }
        }
    }
}