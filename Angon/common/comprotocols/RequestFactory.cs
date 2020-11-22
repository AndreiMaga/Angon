using Angon.common.comprotocols.requests;
using Angon.common.headers;

namespace Angon.common.comprotocols
{
    class RequestFactory
    {
        internal static Request Factory(WraperHeader wh)
        {
            switch (wh.Type)
            {

                case 'C':
                    return new ClientHello(wh.Data);

                case 'P':
                    return new OrderPost(wh.Data);
                case 'S':
                    return new ServerHello(wh.Data);

                default:
                    return new Request(wh.Data);
            }
        }
    }
}
