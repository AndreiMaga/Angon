using Angon.common.comprotocols.requests;
using Angon.common.config;
using Angon.common.headers;
using Angon.common.storage;
using Angon.common.utils;
using Serilog;
using System;
using System.IO;
using System.Net.Sockets;

namespace Angon.common.runner.runners
{
    class OrderReciever
    {

        public static void ReicieveClientHello(RequestWithHeader<ClientHelloHeader> ch, string sha)
        {
            Recieve(ch.header.SizeInBytes, ch.Client.GetStream(), sha);
            // Register order to database
            StorageProvider.GetInstance().ClientRegisteredOrder(ch, sha);
        }

        public static void RecieveJob(RequestWithHeader<JobHeader> jr)
        {
            Recieve(jr.header.Size, jr.Client.GetStream(), jr.header.JobID);

        }

        public static void Recieve(long size, NetworkStream stream, string sha)
        {
            string path = ConfigReader.GetInstance().Config.SavePath + "\\" + sha;
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (IOException)
            {

            }

            byte[] dataArray = new byte[ConfigReader.GetInstance().Config.ReadSize];

            Log.Information("Writing zip file of {0} bytes!", size);

            while (size > 0)
            {
                int readTo = size > dataArray.Length ? dataArray.Length : (int)size;
                try
                {
                    stream.Read(dataArray, 0, readTo);
                }
                catch (Exception)
                {
                    Log.Warning("Something happened, aborting!");
                    CleanUp(path);
                    return;
                }

                ByteArrayUtils.ByteArrayToFile(dataArray, path + "\\temp.zip"); // write data array to temp file

                size -= readTo;
            }

            Log.Information("Finished writing the zip file to {0}", path);

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