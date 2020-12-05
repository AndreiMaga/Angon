using Angon.common.headers;
using Angon.common.utils;
using Serilog;
using System;
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
    }
}
