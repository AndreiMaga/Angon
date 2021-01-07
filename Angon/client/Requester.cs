using Angon.common.comprotocols.requests;
using Angon.common.config;
using Angon.common.headers;
using Angon.common.reciever;
using Angon.common.sender;
using Angon.common.storage;
using Angon.common.utils;
using Serilog;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;

namespace Angon.client
{
    /// <summary>
    /// Client side requester
    /// </summary>
    class Requester
    {

        public static void RequestResultFromMaster(string sha)
        {
            RequestFinishedOrderHeader rfo = new RequestFinishedOrderHeader()
            {
#if DEBUG
                ClientToken = "",
#else
                ClientToken = StorageProvider.GetInstance().GetClientsToken(),
#endif
                Ip = ConfigReader.GetInstance().Config.PredefinedIP == "" ? Networking.GetIPAddress() : ConfigReader.GetInstance().Config.PredefinedIP,
                Sha = sha
            };

            WraperHeader wh = new WraperHeader()
            {
                Data = ByteArrayUtils.ToByteArray(rfo),
                Type = HeaderTypes.RequestFinishedOrderHeader
            };
            TcpClient client = new TcpClient(ConfigReader.GetInstance().Config.Ip, ConfigReader.GetInstance().Config.Port);
            Sender.Send(wh, client);

            new Reciever().ProcessClient(client);

        }

        /// <summary>
        /// Start the ordering process with the two folders and creates a zip file with the parameters
        /// </summary>
        /// <param name="exeDirectoryPath">The path to the exe directory</param>
        /// <param name="inputDirectoryPath">The path to the input directory</param>
        public static void RunFromFolders(string exeDirectoryPath, string inputDirectoryPath)
        {
            string basePath = Path.Combine(ConfigReader.GetInstance().Config.SavePath, "zipToSend");

            try
            {
                Directory.Delete(Path.Combine(basePath, "temp", "exe"), true);
#if DEBUG
                Log.Debug("Deleting {0}", Path.Combine(basePath, "temp", "exe"));
#endif
            }
            catch (IOException)
            {
                // if the directory does not exist, ignore
            }

            try
            {
                Directory.Delete(Path.Combine(basePath, "temp", "input"), true);
#if DEBUG
                Log.Debug("Deleting {0}", Path.Combine(basePath, "temp", "input"));
#endif
            }
            catch (IOException)
            {
                // if the directory does not exist, ignore
            }

            try
            {
                Directory.CreateDirectory(Path.Combine(basePath, "temp"));
            }
            catch (IOException)
            {
                // if the directory already exsists, ignore
            }

            try
            {
                File.Delete(Path.Combine(basePath, "temp.zip"));
#if DEBUG
                Log.Debug("Deleting {0}", Path.Combine(basePath, "temp.zip"));
#endif
            }
            catch (IOException)
            {
                // if the zip already exsists, ignore
            }


            IOUtils.DirectoryCopy(exeDirectoryPath, Path.Combine(basePath, "temp", "exe"), true);
            IOUtils.DirectoryCopy(inputDirectoryPath, Path.Combine(basePath, "temp", "input"), true);

            ZipFile.CreateFromDirectory(Path.Combine(basePath, "temp"), Path.Combine(basePath, "temp.zip"));
            Log.Information("Created the zip");

            SendClientHello(CreateClientHello(Path.Combine(basePath, "temp.zip")));
        }

        /// <summary>
        /// Creates <see cref="ClientHelloHeader"/> from the <paramref name="path"/>
        /// </summary>
        /// <param name="path">The path to the zip file to be sent</param>
        /// <returns></returns>
        public static ClientHelloHeader CreateClientHello(string path) => new ClientHelloHeader
        {
            ClientUTCTime = DateTime.UtcNow,
            ClientIP = ConfigReader.GetInstance().Config.PredefinedIP == "" ? Networking.GetIPAddress() : ConfigReader.GetInstance().Config.PredefinedIP,
            ClientVersion = ConfigReader.GetInstance().Config.Version,
            SizeInBytes = new FileInfo(path).Length,
#if DEBUG
            ClientToken = ""
#else
            ClientToken = StorageProvider.GetInstance().GetClientsToken()
#endif
        };

        /// <summary>
        /// Sends <paramref name="chh"/> to the server
        /// </summary>
        /// <param name="chh">The header to be sent</param>
        public static void SendClientHello(ClientHelloHeader chh)
        {
            RequestWithHeader<ClientHelloHeader> gh = new RequestWithHeader<ClientHelloHeader>(ByteArrayUtils.ToByteArray(chh));

            WraperHeader wraperHeader = new WraperHeader
            {
                Type = HeaderTypes.ClientHelloHeader,
                Data = gh.Data
            };

            StorageProvider.GetInstance().ClientRegisteredOrder(gh, "");

            TcpClient serverConnection = new TcpClient(ConfigReader.GetInstance().Config.Ip, ConfigReader.GetInstance().Config.Port);

            Sender.Send(wraperHeader, serverConnection);

            new Reciever().ProcessClient(serverConnection);

        }
    }
}
