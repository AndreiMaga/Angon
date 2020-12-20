using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Angon.common.headers
{
    [Serializable]
    class ClientHelloHeader : ISerializable
    {
        DateTime clientUTCTime;
        string clientIP;
        long sizeInBytes;
        string clientVersion;
        string clientToken;

        public DateTime ClientUTCTime { get => clientUTCTime; set => clientUTCTime = value; }
        public string ClientIP { get => clientIP; set => clientIP = value; }
        public long SizeInBytes { get => sizeInBytes; set => sizeInBytes = value; }
        public string ClientVersion { get => clientVersion; set => clientVersion = value; }
        public string ClientToken { get => clientToken; set => clientToken = value; }

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
