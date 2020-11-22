using System;
using System.Runtime.Serialization;

namespace Angon.common.headers
{
    /// <summary>
    /// Server Hello Header, this will be sent to the client after recieving <see cref="ClientHelloHeader"/>
    /// </summary>
    [Serializable]
    class ServerHelloHeader : ISerializable
    {
        /// <summary>
        /// Is the request accepted or not, for details <see cref="Angon.common.runner.runners.ClientHelloRunner"/>
        /// </summary>
        public bool AcceptedRequest { get; set; }

        /// <summary>
        /// String representation of the order's <see cref="SHA256"/>
        /// </summary>
        public string Sha { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ServerHelloHeader() { }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        public ServerHelloHeader(SerializationInfo info, StreamingContext context)
        {
            AcceptedRequest = (bool)info.GetValue("AcceptedRequest", typeof(bool));
            Sha = (string)info.GetValue("Sha", typeof(string));
        }

        /// <summary>
        /// Serialization method
        /// </summary>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("AcceptedRequest", AcceptedRequest, typeof(bool));
            info.AddValue("Sha", Sha, typeof(string));
        }
    }
}
