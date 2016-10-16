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

            jobhostConfiguration.UseFtp();

            using (var host = new JobHost(jobhostConfiguration))
            {
                host.Call(typeof(Functions).GetMethod("SendFileToFtps"));
            }
        }
    }
}