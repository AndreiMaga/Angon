using Angon.common.utils;

namespace Angon.common.comprotocols.requests
{
    class GenericHello<T> : Request
    {
        public T header;
        public GenericHello(byte[] data) : base(data)
        {
            header = ByteArrayUtils.FromByteArray<T>(data);
        }
    }
}
