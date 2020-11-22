using System.Net.Sockets;

namespace Angon.common.comprotocols
{
    /// <summary>
    /// Base class for <see cref="comprotocols.requests"/>
    /// </summary>
    class Request
    {
        /// <summary>
        /// Same as <see cref="headers.WraperHeader.data"/>
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// The sender
        /// </summary>
        public TcpClient Client { get; set; }

        /// <summary>
        /// Constructor that takes <see cref="headers.WraperHeader.data"/>
        /// </summary>
        /// <param name="data"></param>
        public Request(byte[] data)
        {
            Data = data;
        }

    }
}
