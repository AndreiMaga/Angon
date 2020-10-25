using Angon.common.comprotocols;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Angon.master.server
{
    class Reciever
    {
        
        public async void ProcessClient(TcpClient client)
        {
            byte[] bytes = new byte[256];
            List<byte> data = new List<byte>();
            int i;
            // get a stream going
            NetworkStream stream = client.GetStream();

            // read the input from a client
            while ((i = await stream.ReadAsync(bytes, 0, bytes.Length)) != 0)
            {
                data.AddRange(bytes);
            }

            ProcessData(data);

            client.Close();
        }


        public String ProcessData(List<byte> data)
        {
            Request req = RequestFactory.Factory(data);

            return ProcessRequest(req);
        }

        public String ProcessRequest(Request req)
        {

            return "";
        }
    }
}
