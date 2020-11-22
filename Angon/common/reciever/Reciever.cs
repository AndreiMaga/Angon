using Angon.common.comprotocols;
using Angon.common.headers;
using Angon.common.runner.runners;
using Angon.common.utils;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Angon.common.reciever
{
    /// <summary>
    /// This will recieve as both sides and continue by calling runners
    /// </summary>
    class Reciever
    {
        /// <summary>
        /// Processes a <see cref="TcpClient"/> and recieves data from it
        /// </summary>
        /// <param name="client">the client to recieve data from</param>
        public void ProcessClient(TcpClient client)
        {
            byte[] bytes = new byte[512];
            List<byte> data = new List<byte>();

            NetworkStream stream = client.GetStream();

            byte[] sizeArray = new byte[sizeof(int)]; // read a long that denotes the size of the size to read

            stream.Read(sizeArray, 0, sizeof(int)); // read the first 4 bytes from the stream

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

        /// <summary>
        /// Continuation of <see cref="ProcessClient"/>
        /// </summary>
        /// <param name="data">List of bytes read from the connection</param>
        /// <param name="client">the client that it was read from</param>
        public void ProcessData(List<byte> data, TcpClient client)
        {
            WraperHeader wh = ByteArrayUtils.FromByteArray<WraperHeader>(data.ToArray());
            Request req = RequestFactory.Factory(wh);
            req.Client = client;
            Runner.Start(req);

        }
    }
}
