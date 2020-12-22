using Angon.common.utils;

namespace Angon.common.comprotocols.requests
{
    class RequestWithHeader<T> : Request
    {
        public T header;
        public RequestWithHeader(byte[] data) : base(data)
        {
            header = ByteArrayUtils.FromByteArray<T>(data);
        }
    }
}
