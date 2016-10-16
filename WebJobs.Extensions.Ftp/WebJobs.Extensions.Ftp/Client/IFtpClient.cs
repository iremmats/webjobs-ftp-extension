using System.IO;
using System.Threading.Tasks;

namespace WebJobs.Extensions.Ftp.Client
{
    public interface IFtpClient
    {
        Task SendFileAsync(string path, Stream data);
    }
}