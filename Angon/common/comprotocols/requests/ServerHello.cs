using Angon.common.headers;
using Angon.common.utils;

namespace Angon.common.comprotocols.requests
{
    /// <summary>
    /// Server Hello Request
    /// </summary>
    class ServerHello : Request
    {
        /// <summary>
        /// <see cref="ServerHelloHeader"/>
        /// </summary>
        public ServerHelloHeader header;

        /// <summary>
        /// Constructor that takes <see cref="headers.WraperHeader.data"/> 
        /// and transforms it to the right type of Header
        /// </summary>
        /// <param name="data"><see cref="headers.WraperHeader.data"/></param>
        public ServerHello(byte[] data) : base(data)
        {
            header = ByteArrayUtils.FromByteArray<ServerHelloHeader>(data);
        }
    }

}
