using Angon.common.comprotocols;
using Angon.common.config;
using Angon.common.headers;
using Angon.common.runner;
using Angon.common.utils;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
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

            if (!CheckConnections(client))
            {
                client.Close();
                return;
            }

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
            if (!CheckRequest(wh))
            {
                return;
            }
            Request req = RequestFactory.Factory(wh);
            req.Client = client;
            Runner.Start(req);

        }

        public bool CheckConnections(TcpClient client)
        {
            // slaves will only allow connections from the masters ip as defined in config
            if (ConfigReader.GetInstance().Config.Type == 1)
            {
                if ((client.Client.RemoteEndPoint as IPEndPoint).Address.ToString() != ConfigReader.GetInstance().Config.Ip)
                {
                    return false;
                }
            }
            // if the master has RestrictUnknownConnections active, only allow ip's defined in KnownIPs
            if (ConfigReader.GetInstance().Config.Type == 0)
            {
                if (ConfigReader.GetInstance().Config.RestrictUnknownConnections)
                {
                    if(!ConfigReader.GetInstance().Config.KnownIPs.Contains((client.Client.RemoteEndPoint as IPEndPoint).Address.ToString()))
                    {
                        return false;
                    }
                }
            }
            // if everything is good
            return true;
        }

        public bool CheckRequest(WraperHeader wh)
        {
            return AllowedRequests.GetType(ConfigReader.GetInstance().Config.Type).Contains(wh.Type);
        }
    }
}
