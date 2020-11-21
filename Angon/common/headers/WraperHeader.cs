using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angon.common.headers
{
    class WraperHeader
    {
        char type;
        byte[] data;

        public char Type { get => type; set => type = value; }
        public byte[] Data { get => data; set => data = value; }

    }
}
