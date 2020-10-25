using Angon.common.zip;
using System.Collections.Generic;


namespace Angon.common.comprotocols
{
    internal class ProcessRequest : Request
    {

        public ProcessRequest(List<byte> data) : base(data)
        {
            // expects 'P' then a zip file as List<byte>
            data.Remove((byte)('P')); // delete the request type
            ParseRequest();
        }

        public new void ParseRequest()
        {
            ZipUtils.ByteArrayToFile("tempfile.zip", this.Data.ToArray());
        }
    }
}
