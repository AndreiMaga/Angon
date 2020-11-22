using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Angon.common.headers
{
    [Serializable]
    class WraperHeader : ISerializable
    {
        char type;
        byte[] data;

        public char Type { get => type; set => type = value; }
        public byte[] Data { get => data; set => data = value; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Type", type, typeof(char));
            info.AddValue("Data", data, typeof(byte[]));
        }

        public WraperHeader() { }
        public WraperHeader(SerializationInfo info, StreamingContext context)
        {
            type = (char)info.GetValue("Type", typeof(char));
            data = (byte[])info.GetValue("Data", typeof(byte[]));
        }
    }
}
