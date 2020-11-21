using Angon.common.comprotocols.requests;
using Angon.common.config;
using Angon.common.storage;
using Angon.common.utils;
using System.IO;

namespace Angon.common.runner.runners
{
    class OrderReciever
    {
        public static void Recieve(ClientHello ch, string sha)
        {
            string path = ConfigReader.GetInstance().Config.SavePath + "/" + sha;
            Directory.CreateDirectory(path);

            // This will make an object with the ulong stored.
            byte[] dataArray = new byte[ConfigReader.GetInstance().Config.ReadSize];
            // create an array of size described in the config , this will eat up a lot of memory if the 

            long size = ch.header.SizeInBytes;
            while (size > 0)
            {
                int readTo = size > dataArray.Length ? dataArray.Length : (int)size;
                ch.Client.GetStream().Read(dataArray, 0, readTo);

                ByteArrayUtils.ByteArrayToFile(dataArray, path + "/temp.zip"); // write data array to temp file

                size -= readTo;
            }
            // Register order to database
            StorageProvider.GetInstance().ClientRegisteredOrder(ch, sha);
        }
    }
}