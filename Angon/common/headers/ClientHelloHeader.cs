using System;

namespace Angon.common.headers
{
    class ClientHelloHeader
    {
        DateTime clientUTCTime;
        string clientIP;
        long sizeInBytes;
        string clientVersion;

        public DateTime ClientUTCTime { get => clientUTCTime; set => clientUTCTime = value; }
        public string ClientIP { get => clientIP; set => clientIP = value; }
        public long SizeInBytes { get => sizeInBytes; set => sizeInBytes = value; }
        public string ClientVersion { get => clientVersion; set => clientVersion = value; }
    }
}
