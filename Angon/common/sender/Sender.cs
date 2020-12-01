using Angon.common.headers;
using Angon.common.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Angon.common.sender
{
    class Sender
    {

        public static void Send(WraperHeader header, TcpClient client)
        {
            byte[] byteArray = ByteArrayUtils.ToByteArray(header);
            Console.WriteLine("Sending {0} bytes!", byteArray.Length);
            client.GetStream().Write(BitConverter.GetBytes(byteArray.Length), 0, sizeof(int));
            client.GetStream().Write(byteArray, 0, byteArray.Length);
        }
    }
}
