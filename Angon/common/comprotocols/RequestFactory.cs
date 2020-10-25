using System.Collections.Generic;

namespace Angon.common.comprotocols
{
    class RequestFactory
    {
        internal static Request Factory(List<byte> data)
        {
            switch (data[0])
            {
                case (byte)'P':
                    return new ProcessRequest(data);

                default:
                    return new Request(data);
            }
        }
    }
}
