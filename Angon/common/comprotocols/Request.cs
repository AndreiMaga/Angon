using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Angon.common.comprotocols
{
    class Request
    {
        public byte[] Data { get; set; }
        public TcpClient Client { get; set; }
        public Request(byte[] data)
        {
            Data = data;
        }

    }
}
