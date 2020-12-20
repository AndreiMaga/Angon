using System;
using System.Reflection;
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
        public char Type { get; set; }

        /// <summary>
        /// All the other headers as bytes
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public WraperHeader() { }

        /// <summary>
        /// Deserialization Constructor
        /// </summary>
        public WraperHeader(SerializationInfo info, StreamingContext context)
        {
            foreach (PropertyInfo fi in GetType().GetProperties())
            {
                fi.SetValue(this, info.GetValue(fi.Name, fi.PropertyType));
            }
        }

        /// <summary>
        /// Serialization method
        /// </summary>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (PropertyInfo fi in GetType().GetProperties())
            {
                info.AddValue(fi.Name, fi.GetValue(this), fi.PropertyType);
            }
        }
    }
}
