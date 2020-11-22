using System;
using System.Runtime.Serialization;

namespace Angon.common.headers
{
    /// <summary>
    /// Wrapper Header for all the other headers
    /// </summary>
    [Serializable]
    class WraperHeader : ISerializable
    {
        /// <summary>
        /// The type of the header that it's wrapped inside <see cref="data"/>
        /// </summary>
        char type;

        /// <summary>
        /// All the other headers as bytes
        /// </summary>
        byte[] data;

        /// <summary>
        /// Getter/Setter for <see cref="type"/>
        /// </summary>
        public char Type { get => type; set => type = value; }

        /// <summary>
        /// Getter/Setter for <see cref="data"/>
        /// </summary>
        public byte[] Data { get => data; set => data = value; }

        /// <summary>
        /// Serialization method
        /// </summary>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Type", type, typeof(char));
            info.AddValue("Data", data, typeof(byte[]));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public WraperHeader() { }

        /// <summary>
        /// Deserialization Constructor
        /// </summary>
        public WraperHeader(SerializationInfo info, StreamingContext context)
        {
            type = (char)info.GetValue("Type", typeof(char));
            data = (byte[])info.GetValue("Data", typeof(byte[]));
        }
    }
}
