using Angon.common.comprotocols.requests;
using Angon.common.config;
using Angon.common.headers;
using Angon.common.storage;
using Angon.common.utils;
using Angon.master.scheduler;
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

        public static void RecieveFinishedOrder(RequestWithHeader<FinishedOrderHeader> foh)
        {
            Recieve(foh.header.Size, foh.Client.GetStream(), Path.Combine(ConfigReader.GetInstance().Config.SavePath, foh.header.Message));
        }

        public static void RecieveFinishedJob(RequestWithHeader<JobResultHeader> jrh)
        {
            Recieve(jrh.header.Size, jrh.Client.GetStream(), Path.Combine(Scheduler.GetOrderPath, "result", jrh.header.JobID));
        }

        public static void Recieve(long size, NetworkStream stream, string sha, bool resultsave = false)
        {
            string path = Path.Combine(ConfigReader.GetInstance().Config.SavePath, sha);

            if (resultsave)
            {
                path = Path.Combine(ConfigReader.GetInstance().Config.SavePath, Scheduler.Order.Sha, "result");
            }

            try
            {
                Directory.CreateDirectory(path);
            }
            catch (IOException)
            {

            }

            byte[] dataArray = new byte[ConfigReader.GetInstance().Config.ReadSize];

            Log.Information("Writing zip file of {0} bytes!", size);

            string savepath = Path.Combine(path, "temp.zip");
            if (resultsave)
            {
                savepath = Path.Combine(path, sha + ".zip");
            }
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

                ByteArrayUtils.ByteArrayToFile(dataArray, savepath); // write data array to temp file

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