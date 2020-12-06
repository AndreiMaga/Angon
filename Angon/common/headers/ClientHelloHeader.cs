using System;
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
            clientUTCTime = (DateTime)info.GetValue("ClientUTCTime", typeof(DateTime));
            clientIP = (string)info.GetValue("ClientIP", typeof(string));
            sizeInBytes = (long)info.GetValue("SizeInBytes", typeof(long));
            clientVersion = (string)info.GetValue("ClientVersion", typeof(string));
            clientToken = (string)info.GetValue("ClientToken", typeof(string));


        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ClientUTCTime", ClientUTCTime, typeof(DateTime));
            info.AddValue("ClientIP", ClientIP, typeof(string));
            info.AddValue("SizeInBytes", SizeInBytes, typeof(long));
            info.AddValue("ClientVersion", ClientVersion, typeof(string));
            info.AddValue("ClientToken", ClientToken, typeof(string));
        }
    }
}
