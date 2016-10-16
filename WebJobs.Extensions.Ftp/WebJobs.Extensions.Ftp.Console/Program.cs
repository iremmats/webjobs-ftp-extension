using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebJobs.Extensions.Ftp.Config;
using WebJobs.Extensions.Ftp.Listener;

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