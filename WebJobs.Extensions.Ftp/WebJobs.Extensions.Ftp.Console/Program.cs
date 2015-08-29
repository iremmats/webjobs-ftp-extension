using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace WebJobs.Extensions.Ftp.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            JobHostConfiguration config = new JobHostConfiguration();


            config.Tracing.ConsoleLevel = TraceLevel.Verbose;
            config.StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=SECRET;AccountKey=JHtJ/ACL8tOwGcXA4NhlE+xMi6Z2myBoc/98e59n2okV9B7ebVgyRd6a2VlmzL9lBALl5qxaGBa5E/9OtWXDNg==";
            config.DashboardConnectionString = "DefaultEndpointsProtocol=https;AccountName=SECRET;AccountKey=JHtJ/ACL8tOwGcXA4NhlE+xMi6Z2myBoc/98e59n2okV9B7ebVgyRd6a2VlmzL9lBALl5qxaGBa5E/9OtWXDNg==";

            config.UseFtp();




            JobHost host = new JobHost(config);
            
            host.RunAndBlock();
        }

    }
}
