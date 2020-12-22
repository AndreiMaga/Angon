using Angon.common.comprotocols.requests;
using Angon.common.config;
using Angon.common.headers;
using Angon.common.sender;
using Angon.common.storage;
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
            if (!ch.header.AcceptedRequest)
            {
                // there is one pending, check status
                Log.Warning(ch.header.Message);
                StorageProvider.GetInstance().DeleteLatestClientOrder();
                return;
            }

            if (ch.header.Message.Contains("Token:"))
            {
                // the server generated a new token for the client
                // store it in the database
                StorageProvider.GetInstance().RegisterClientToken("localhost", ch.header.Message.Split(':')[1]);
            }

            Log.Information("Recieved sha {0}!", ch.header.Sha);
            // if the order request was accepted
            // send the zip
            string path = ConfigReader.GetInstance().Config.SavePath + "\\zipToSend\\temp.zip";

            Sender.SendZip(path, ch.Client.GetStream());

            // Done, now register the sha to the local database for future operations
            StorageProvider.GetInstance().UpdateSha(ch.header.Sha);
        }
    }
}
