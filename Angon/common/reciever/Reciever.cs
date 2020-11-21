using Angon.common.comprotocols;
using Angon.common.headers;
using Angon.common.runner.runners;
using Angon.common.utils;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Angon.common.reciever
{
    class Reciever
    {
        public async void ProcessClient(TcpClient client)
        {
            byte[] bytes = new byte[256];
            List<byte> data = new List<byte>();

            NetworkStream stream = client.GetStream();

            // I don't think this works as intended!!!
            while ((await stream.ReadAsync(bytes, 0, bytes.Length)) != 0)
            {
                data.AddRange(bytes);
            }

            ProcessData(data, client);
            stream.Close();
            client.Close();
        }

        public void ProcessData(List<byte> data, TcpClient client)
        {
            WraperHeader wh = ByteArrayUtils.FromByteArray< WraperHeader>(data.ToArray());
            Request req = RequestFactory.Factory(wh);
            req.Client = client;
            Runner.Start(req);
            
        }
    }
}
