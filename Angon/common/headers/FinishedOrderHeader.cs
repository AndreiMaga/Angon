using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Angon.common.headers
{
    [Serializable]
    class FinishedOrderHeader : ISerializable
    {
        public long Size { get; set; }
        public string Message { get; set; }

        public FinishedOrderHeader() { }
        public FinishedOrderHeader(SerializationInfo info, StreamingContext context)
        {
            foreach (PropertyInfo fi in GetType().GetProperties())
            {
                fi.SetValue(this, info.GetValue(fi.Name, fi.PropertyType));
            }

        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (PropertyInfo fi in GetType().GetProperties())
            {
                info.AddValue(fi.Name, fi.GetValue(this), fi.PropertyType);
            }
        }
    }
}
