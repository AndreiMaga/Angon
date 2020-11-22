using Angon.common.headers;
using Angon.common.utils;

namespace Angon.common.comprotocols.requests
{
    /// <summary>
    /// Client Hello Request
    /// </summary>
    class ClientHello : Request
    {
        /// <summary>
        /// <see cref="ClientHelloHeader"/>
        /// </summary>
        public ClientHelloHeader header;

        /// <summary>
        /// Constructor that takes <see cref="headers.WraperHeader.data"/> 
        /// and transforms it to the right type of Header
        /// </summary>
        /// <param name="data"><see cref="headers.WraperHeader.data"/></param>
        public ClientHello(byte[] data) : base(data)
        {
            header = ByteArrayUtils.FromByteArray<ClientHelloHeader>(data);
        }
    }
}
