using Angon.common.headers;
using Angon.common.utils;
using System;

namespace Angon.common.comprotocols.requests
{
    class JobRequest : Request
    {
        public JobHeader header;
        public JobRequest(byte[] data) : base(data)
        {
            header = ByteArrayUtils.FromByteArray<JobHeader>(data);
        }
    }
}
