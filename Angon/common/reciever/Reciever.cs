using Angon.common.comprotocols;
using Angon.common.headers;
using Angon.common.runner.runners;
using Angon.common.utils;
using Serilog;
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
            List<byte> data = new List<byte>();

            NetworkStream stream = client.GetStream();

            byte[] sizeArray = new byte[sizeof(int)];

            stream.Read(sizeArray, 0, sizeof(int));

            int size = BitConverter.ToInt32(sizeArray, 0);
            byte[] bytes = new byte[size];

            Log.Information("Will recieve {0} bytes!", size);
            stream.Read(bytes, 0, size);
            data.AddRange(bytes);
            Log.Information("Recieved {0} bytes", data.Count);

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
