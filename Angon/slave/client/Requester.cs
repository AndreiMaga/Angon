using Angon.common.config;
using Angon.common.headers;
using Angon.common.reciever;
using Angon.common.utils;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Angon.slave.client
{

    class Requester
    {
        // This will be run when a Client needs to send data to the Master to be run.
        public static void RunFromFolders(string exeDirectoryPath, string inputDirectoryPath)
        {
            string basePath = ConfigReader.GetInstance().Config.SavePath + "/zipToSend";
            File.Move(exeDirectoryPath, basePath + "/temp/exe");
            File.Move(inputDirectoryPath, basePath + "/temp/input");

            ZipFile.CreateFromDirectory(basePath+"/temp", basePath+"/temp.zip");

            File.Delete(basePath+"/temp/exe");
            File.Delete(basePath + "/temp/input");

            SendClientHello(CreateClientHello(basePath + "/temp.zip"));
        }

        public static ClientHelloHeader CreateClientHello(string path) => new ClientHelloHeader
        {
            ClientUTCTime = DateTime.UtcNow,
            ClientIP = ConfigReader.GetInstance().Config.PredefinedIP == "" ? Networking.GetIPAddress() : ConfigReader.GetInstance().Config.PredefinedIP,
            ClientVersion = ConfigReader.GetInstance().Config.Version,
            SizeInBytes = new FileInfo(path).Length
        };

        public static void SendClientHello(ClientHelloHeader chh)
        {
            WraperHeader wraperHeader = new WraperHeader
            {
                Type = 'C',
                Data = ByteArrayUtils.ToByteArray(chh)
            };
            byte[] byteArray = ByteArrayUtils.ToByteArray(wraperHeader);

            TcpClient serverConnection = new TcpClient(ConfigReader.GetInstance().Config.Ip, ConfigReader.GetInstance().Config.Port);
            serverConnection.GetStream().Write(byteArray, 0, byteArray.Length); // Write Client Hello

            Task task = new Task(() =>
            {
                new Reciever().ProcessClient(serverConnection);
            });
            task.Start();
            task.Wait(); // Wait for the task to be done aka make it not async, as the client does not have to be async

        }
    }
}
