using AlexPilotti.FTPS.Client;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WebJobs.Extensions.Ftp.Config;

namespace WebJobs.Extensions.Ftp.Client
{
    public sealed class FtpClient : IFtpClient
    {
        private readonly FtpConfiguration _config;

        public FtpClient(FtpConfiguration config)
        {
            _config = config;
        }




        public async Task SendFileAsync(string path, Stream data)
        {
            using (var client = new FTPSClient())
            {
                client.Connect(_config.Host.Host,
                    new NetworkCredential(_config.Username,
                        _config.Password),
                    ESSLSupportMode.CredentialsRequired |
                    ESSLSupportMode.DataChannelRequested);

                var ftps = client.PutFile(path);

                var bytes = ReadToEnd(data);
                await ftps.WriteAsync(bytes, 0, bytes.Length);
                ftps.Close();
            }
        }

        private byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}