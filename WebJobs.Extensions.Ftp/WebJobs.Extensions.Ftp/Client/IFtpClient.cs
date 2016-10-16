using System.IO;
using System.Threading.Tasks;
using WebJobs.Extensions.Ftp.Model;

namespace WebJobs.Extensions.Ftp.Client
{
    public interface IFtpClient
    {
        Task SendFileAsync(FtpMessage ftpMessage);
    }
}