using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJobs.Extensions.Ftp
{
    public class FtpConfiguration
    {
        public FtpConfiguration(TimeSpan pollingInterval)
        {
            PollingInterval = pollingInterval;
        }

        private TimeSpan PollingInterval { get; set; }

    }
}
