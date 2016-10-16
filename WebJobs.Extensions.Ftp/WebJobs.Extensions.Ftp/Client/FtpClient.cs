using AlexPilotti.FTPS.Client;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Renci.SshNet;
using WebJobs.Extensions.Ftp.Config;
using WebJobs.Extensions.Ftp.Model;

namespace WebJobs.Extensions.Ftp.Client
{
    public sealed class FtpClient : IFtpClient
    {
        private readonly FtpConfiguration _config;

        

        public FtpClient(FtpConfiguration config)
        {
            _config = config;
        }
        
        public async Task SendFileAsync(FtpMessage ftpMessage)
        {
            switch (ftpMessage.FtpHost.Scheme)
            {
                case "sftp":
                    await SendBySftp(ftpMessage);
                    break;
                case "ftp":
                    await SendByFtps(ftpMessage);
                    break;
                default: throw new ArgumentException("Unsupported uri scheme. Only ftps and sftp is supported.", nameof(ftpMessage));
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

        private async Task SendBySftp(FtpMessage ftpMessage)
        {
            var host = ftpMessage.FtpHost;
            var path = ftpMessage.Filename;
            var username = ftpMessage.Username;
            var password = ftpMessage.Password;
            var port = ftpMessage.FtpPort;

            using (var sftpClient = new SftpClient(host.Host, port, username, password))
            {
                sftpClient.Connect();
                //sftpClient.ChangeDirectory("tmp");
                sftpClient.UploadFile(ftpMessage.Data, path);
                sftpClient.Disconnect();
            }
        }

        private async Task SendByFtps(FtpMessage ftpMessage)
        {
            var host = ftpMessage.FtpHost;
            var path = ftpMessage.Filename;
            var username = ftpMessage.Username;
            var password = ftpMessage.Password;
            var port = ftpMessage.FtpPort;


            using (var client = new FTPSClient())
            {
                client.Connect(host.Host,
                    new NetworkCredential(username, password),
                    ESSLSupportMode.CredentialsRequired |
                    ESSLSupportMode.DataChannelRequested);

                var ftps = client.PutFile(path);

                var bytes = ReadToEnd(ftpMessage.Data);
                await ftps.WriteAsync(bytes, 0, bytes.Length);
                ftps.Close();
            }
        }
    }
}