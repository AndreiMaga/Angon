using System;
using System.Runtime.Serialization;

namespace Angon.common.headers
{
    [Serializable]
    class ServerAvailableHeader : ISerializable
    {
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }
        public ServerAvailableHeader() { }
        public ServerAvailableHeader(SerializationInfo info, StreamingContext context)
        {

        }
    }
}
