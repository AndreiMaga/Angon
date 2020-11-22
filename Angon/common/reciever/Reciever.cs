using Angon.common.comprotocols;
using Angon.common.headers;
using Angon.common.runner.runners;
using Angon.common.utils;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Angon.common.reciever
{
    class Reciever
    {
        public void ProcessClient(TcpClient client)
        {
            byte[] bytes = new byte[512];
            List<byte> data = new List<byte>();

            NetworkStream stream = client.GetStream();

            byte[] sizeArray = new byte[sizeof(int)]; // read a long that denotes the size of the size to read

            stream.Read(sizeArray, 0, sizeof(int)); // read the first 8 bytes from the stream

            //if (BitConverter.IsLittleEndian)
            //    Array.Reverse(sizeArray);
            int size = BitConverter.ToInt32(sizeArray, 0);
            int osize = size;
            int readSize = (int)(size % bytes.Length);


            Console.WriteLine(String.Format("Will recieve {0} bytes!", size));
            while (readSize != 0 && (stream.Read(bytes, 0, readSize)) != 0)
            {
                data.AddRange(bytes);
                size -= readSize;
                readSize = (int)(size % bytes.Length);
            }
            data.RemoveRange(osize, data.Count - osize);
            Console.WriteLine("Recieved {0} bytes", data.Count);
            ProcessData(data, client);
            stream.Close();
            client.Close();
        }

        public void ProcessData(List<byte> data, TcpClient client)
        {
            WraperHeader wh = ByteArrayUtils.FromByteArray<WraperHeader>(data.ToArray());
            Request req = RequestFactory.Factory(wh);
            req.Client = client;
            Runner.Start(req);
            
        }
    }
}
