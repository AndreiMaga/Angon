using Angon.common.headers;
using Angon.common.utils;

namespace Angon.common.comprotocols.requests
{
    class ClientHello : Request
    {
        public ClientHelloHeader header;
        public ClientHello(byte[] data) : base(data)
        {
            header = ByteArrayUtils.FromByteArray<ClientHelloHeader>(data);
        }
    }
}
