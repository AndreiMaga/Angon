using Angon.common.comprotocols.requests;
using Angon.common.config;
using Angon.common.headers;
using Angon.common.sender;
using Angon.common.storage;
using Angon.common.utils;
using System;
using System.IO;
using System.IO.Compression;

namespace Angon.common.runner.runners
{
    class RequestFinishedOrderRunner
    {
        public static void Run(RequestWithHeader<RequestFinishedOrderHeader> rfo)
        {
#if DEBUG
            Tuple<string, bool> auth = new Tuple<string, bool>("", true);
#else
            Tuple<string, bool> auth = Authentification.AuthenticateClient(rfo.header.Ip, rfo.header.ClientToken);
#endif
            FinishedOrderHeader foh = new FinishedOrderHeader();

            bool sendData = auth.Item2 == true && !StorageProvider.GetInstance().ClientHasOrder(rfo.header.Ip);
            string orderPath = Path.Combine(ConfigReader.GetInstance().Config.SavePath, rfo.header.Sha);
            string resultPath = Path.Combine(orderPath, "result");
            string zipPath = Path.Combine(orderPath, "result.zip");
            if (sendData)
            {
                if (!File.Exists(zipPath))
                {
                    ZipFile.CreateFromDirectory(resultPath, zipPath);
                }
                long size = new FileInfo(zipPath).Length;
                foh.Size = size;
                foh.Message = rfo.header.Sha;
            }
            else
            {
                foh.Size = 0;
                foh.Message = auth.Item1 == "" ? "You already have an order in progress" : auth.Item1;
            }


            WraperHeader wh = new WraperHeader()
            {
                Data = ByteArrayUtils.ToByteArray(foh),
                Type = HeaderTypes.FinishedOrderHeader
            };

            Sender.Send(wh, rfo.Client);

            if (sendData)
            {
                Sender.SendZip(zipPath, rfo.Client.GetStream());
            }

        }
    }
}
