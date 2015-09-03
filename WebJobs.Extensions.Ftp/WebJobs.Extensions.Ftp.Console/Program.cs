using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using WebJobs.Extensions.Ftp.Listener;

namespace WebJobs.Extensions.Ftp.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //JobHostConfiguration config = new JobHostConfiguration();


            //config.Tracing.ConsoleLevel = TraceLevel.Verbose;
            //config.StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=slaskdev;AccountKey=EP4Q56s0FbKvG7BAtbJL3mCr3Ti3lwe9zkUCllh331V/mr6WjkjiKHLquNmxz047V/6ZzkcrY5QyfC/9+CcApA==";
            //config.DashboardConnectionString = "DefaultEndpointsProtocol=https;AccountName=SECRET;AccountKey=JHtJ/ACL8tOwGcXA4NhlE+xMi6Z2myBoc/98e59n2okV9B7ebVgyRd6a2VlmzL9lBALl5qxaGBa5E/9OtWXDNg==";
            
            //FtpConfiguration ftpConfiguration = new FtpConfiguration(TimeSpan.FromSeconds(30));

            //config.UseFtp(ftpConfiguration);

            //JobHost host = new JobHost(config);
            
            //host.RunAndBlock();

            FtpFileSystemWatcher watcher = new FtpFileSystemWatcher("ftp://localhost",@"c:\ftp\mats\ut",5,"mats","mats",false,false);
            watcher.StartDownloading();
            System.Console.ReadLine();
        }

    }
}
