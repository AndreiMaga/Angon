using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Angon.common.headers
{
    [Serializable]
    class RequestFinishedOrderHeader : ISerializable
    {
        public string ClientToken { get; set; }
        public string Ip { get; set; }
        public string Sha { get; set; }
        public RequestFinishedOrderHeader() { }
        public RequestFinishedOrderHeader(SerializationInfo info, StreamingContext context)
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
