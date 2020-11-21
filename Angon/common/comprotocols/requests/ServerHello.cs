using Angon.common.headers;
using Angon.common.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angon.common.comprotocols.requests
{
    class ServerHello : Request
    {
        public ServerHelloHeader header;
        public ServerHello(byte[] data) : base(data)
        {
            header = ByteArrayUtils.FromByteArray<ServerHelloHeader>(data);
        }
    }
    
}
