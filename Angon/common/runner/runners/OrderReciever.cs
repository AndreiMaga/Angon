using Angon.common.comprotocols.requests;
using Angon.common.config;
using Angon.common.headers;
using Angon.common.storage;
using Angon.common.utils;
using Serilog;
using System;
using System.IO;

namespace Angon.common.runner.runners
{
    /// <summary>
    /// Class wraping <see cref="OrderReciever.Recieve(ClientHello, string)"/>
    /// Server side
    /// </summary>
    class OrderReciever
    {
        /// <summary>
        /// Recieves the zip file from <seealso cref="ServerHelloRunner.Run(ServerHello)"/>
        /// </summary>
        /// <param name="ch"><see cref="ClientHello"/>the client hello</param>
        /// <param name="sha">the sha given to the order</param>
        public static void Recieve(GenericHello<ClientHelloHeader> ch, string sha)
        {
            string path = ConfigReader.GetInstance().Config.SavePath + "\\" + sha;
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (IOException)
            {

            }

            // This will make an object with the ulong stored.
            byte[] dataArray = new byte[ConfigReader.GetInstance().Config.ReadSize];
            // create an array of size described in the config , this will eat up a lot of memory if the 

            long size = ch.header.SizeInBytes;

            Log.Information("Writing zip file of {0} bytes!", size);

            while (size > 0)
            {
                int readTo = size > dataArray.Length ? dataArray.Length : (int)size;
                try
                {
                    ch.Client.GetStream().Read(dataArray, 0, readTo);
                }
                catch (Exception)
                {
                    Log.Warning("The server lost connection with the client, aborting!");
                    CleanUp(path);
                    return;
                }

                ByteArrayUtils.ByteArrayToFile(dataArray, path + "\\temp.zip"); // write data array to temp file

                size -= readTo;
            }

            Log.Information("Finished writing the zip file to {0}", path);
            // Register order to database
            StorageProvider.GetInstance().ClientRegisteredOrder(ch, sha);
        }

        private static void CleanUp(string path)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException) { }
        }
    }
}