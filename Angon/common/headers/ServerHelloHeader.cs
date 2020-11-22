using System;
using System.Runtime.Serialization;

namespace Angon.common.headers
{
    [Serializable]
    class ServerHelloHeader : ISerializable
    {
        public bool AcceptedRequest { get; set; }
        public string Sha { get; set; }

        public ServerHelloHeader() { }
        public ServerHelloHeader(SerializationInfo info, StreamingContext context)
        {
            AcceptedRequest = (bool)info.GetValue("AcceptedRequest", typeof(bool));
            Sha = (string)info.GetValue("Sha", typeof(string));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("AcceptedRequest", AcceptedRequest, typeof(bool));
            info.AddValue("Sha", Sha, typeof(string));
        }
    }
}
