using Angon.common.config;
using Angon.common.headers;
using Angon.common.utils;
using Serilog;
using System;
using System.IO;
using System.Net.Sockets;

namespace Angon.common.sender
{
    class Sender
    {

        public static void Send(WraperHeader header, TcpClient client)
        {
            byte[] byteArray = ByteArrayUtils.ToByteArray(header);
            Log.Information("Sending {0} bytes!", byteArray.Length);
            client.GetStream().Write(BitConverter.GetBytes(byteArray.Length), 0, sizeof(int));
            client.GetStream().Write(byteArray, 0, byteArray.Length);
        }

        public static void SendZip(string path, NetworkStream stream)
        {
            long size = new FileInfo(path).Length;

            int readSize = size > ConfigReader.GetInstance().Config.WriteSize ?
                ConfigReader.GetInstance().Config.WriteSize :
                (int)size;


            Log.Information("Sending {0} bytes!", size);

            byte[] byteArray = new byte[readSize];
            FileStream fs = File.OpenRead(path);
            while (size > 0)
            {
                fs.Read(byteArray, 0, readSize);
                stream.Write(byteArray, 0, readSize); // send the array
                size -= readSize;
            }
        }
    }
}
