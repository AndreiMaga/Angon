using System.Collections.Generic;

namespace Angon.common.comprotocols
{
    class Request
    {
        public List<byte> Data { get; set; }
        public Request(List<byte> data)
        {
            this.Data = data;
        }

        public void ParseRequest() 
        {

        }

    }
}
