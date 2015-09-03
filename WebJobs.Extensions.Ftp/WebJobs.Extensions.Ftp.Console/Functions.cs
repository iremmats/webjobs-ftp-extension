using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Webjobs.Extensions.Ftp;

namespace WebJobs.Extensions.Ftp.Console
{
    public class Functions
    {
        public static void MyTrigger([FtpTrigger(Path = "in",
            Filemask = "not_implemented_yet")] FtpTriggerValue value)
        {
            System.Console.WriteLine("Filecontent was... " + value.FileContent);
        }
    }
}
