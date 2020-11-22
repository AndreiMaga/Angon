using Angon.common.config;
using Angon.common.headers;
using Angon.common.reciever;
using Angon.common.utils;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Angon.client
{

    class Requester
    {


        // This will be run when a Client needs to send data to the Master to be run.
        public static void RunFromFolders(string exeDirectoryPath, string inputDirectoryPath)
        {
            string basePath = ConfigReader.GetInstance().Config.SavePath + "\\zipToSend";

            try
            {
                Directory.Delete(basePath + "\\temp\\exe");
                

            }catch(IOException)
            {
                // if the directory does not exist, ignore
            }

            try
            {
                Directory.Delete(basePath + "\\temp\\input");
            }
            catch (IOException)
            {
                // if the directory does not exist, ignore
            }

            try
            {
                Directory.CreateDirectory(basePath + "\\temp");

            }
            catch (IOException)
            {
                // if the directory already exsists, ignore
            }


            IOUtils.DirectoryCopy(exeDirectoryPath, basePath + "\\temp\\exe", true);
            IOUtils.DirectoryCopy(inputDirectoryPath, basePath + "\\temp\\input", true);

            ZipFile.CreateFromDirectory(basePath+"/temp", basePath+"\\temp.zip");

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


            Console.WriteLine("Sending {0} bytes to server!", byteArray.Length);
            TcpClient serverConnection = new TcpClient(ConfigReader.GetInstance().Config.Ip, ConfigReader.GetInstance().Config.Port);


            byte[] sizeArray = BitConverter.GetBytes(byteArray.Length);
            serverConnection.GetStream().Write(sizeArray, 0, sizeof(int));
            serverConnection.GetStream().Write(byteArray, 0, byteArray.Length); // Write Client Hello

            Console.WriteLine("Starting the listener");
            Task task = new Task(() =>
            {
                new Reciever().ProcessClient(serverConnection);
            });
            task.Start();
            task.Wait(); // Wait for the task to be done aka make it not async, as the client does not have to be async

        }
    }
}
