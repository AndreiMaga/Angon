using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Angon.common.headers
{
    [Serializable]
    class ClientHelloHeader : ISerializable
    {


        public DateTime ClientUTCTime { get; set; }
        public string ClientIP { get; set; }
        public long SizeInBytes { get; set; }
        public string ClientVersion { get; set; }
        public string ClientToken { get; set; }

        public bool RequestingOrderStatus { get; set; } = false;

        public ClientHelloHeader() { }
        public ClientHelloHeader(SerializationInfo info, StreamingContext context)
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
