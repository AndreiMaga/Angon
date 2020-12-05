using Angon.common.comprotocols.requests;
using Angon.common.config;
using Angon.common.headers;
using Serilog;
using System.IO;

namespace Angon.common.runner.runners
{
    /// <summary>
    /// Runner for when the client recieves Server Hello Header
    /// Client side code
    /// </summary>
    class ServerHelloRunner
    {
        /// <summary>
        /// Runs the logic for when Server Hello Header was recieved
        /// </summary>
        /// <param name="ch"><see cref="ServerHello"/> containing all the necessary information</param>
        public static void Run(GenericHello<ServerHelloHeader> ch)
        {
            Log.Information("Started Server Hello Runner");
            if (!ch.header.AcceptedRequest)
            {
                // there is one pending, check status
                Log.Warning("Client already has order with sha:{0}", ch.header.Sha);
                return;
            }

            // if the order request was accepted
            // send the zip
            string path = ConfigReader.GetInstance().Config.SavePath + "\\zipToSend\\temp.zip";

            long size = new FileInfo(path).Length;

            int readSize = size > ConfigReader.GetInstance().Config.WriteSize ?
                ConfigReader.GetInstance().Config.WriteSize :
                (int)size;

            Log.Information("Recieved sha {0}!", ch.header.Sha);
            Log.Information("Will send {0} bytes!", size);

            byte[] byteArray = new byte[readSize];
            FileStream fs = File.OpenRead(path);
            while (size > 0)
            {
                fs.Read(byteArray, 0, readSize);
                ch.Client.GetStream().Write(byteArray, 0, readSize); // send the array
                size -= readSize;
            }

            // Done, now register the sha to the local database for future operations
        }
    }
}
