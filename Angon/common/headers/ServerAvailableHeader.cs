using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
