using Angon.common.comprotocols.requests;
using Angon.common.headers;

namespace Angon.common.comprotocols
{
    /// <summary>
    /// Wrapper for <see cref="Factory(WraperHeader)"/>
    /// </summary>
    class RequestFactory
    {
        /// <summary>
        /// "Factory" method that creates the right request based on the <see cref="WraperHeader.type"/>
        /// </summary>
        /// <param name="wh"><see cref="WraperHeader"/> header with all the information</param>
        /// <returns><see cref="Request"/> with all the information inside</returns>
        internal static Request Factory(WraperHeader wh)
        {
            switch (wh.Type)
            {

                case 'C':
                    return new ClientHello(wh.Data);

                case 'S':
                    return new ServerHello(wh.Data);

                default:
                    return new Request(wh.Data);
            }
        }
    }
}
