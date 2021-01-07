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
                case HeaderTypes.ClientHelloHeader:
                    return new RequestWithHeader<ClientHelloHeader>(wh.Data);

                case HeaderTypes.ServerHelloHeader:
                    return new RequestWithHeader<ServerHelloHeader>(wh.Data);

                case HeaderTypes.JobHeader:
                    return new RequestWithHeader<JobHeader>(wh.Data);

                case HeaderTypes.ServerAvailableHeader:
                    return new RequestWithHeader<ServerAvailableHeader>(wh.Data);

                case HeaderTypes.JobResultHeader:
                    return new RequestWithHeader<JobResultHeader>(wh.Data);

                case HeaderTypes.RegisterHeader:
                    return new RequestWithHeader<RegisterHeader>(wh.Data);

                case HeaderTypes.RegisterResponseHeader:
                    return new RequestWithHeader<RegisterResponseHeader>(wh.Data);

                case HeaderTypes.RequestFinishedOrderHeader:
                    return new RequestWithHeader<RequestFinishedOrderHeader>(wh.Data);

                case HeaderTypes.FinishedOrderHeader:
                    return new RequestWithHeader<FinishedOrderHeader>(wh.Data);

                default:
                    return new Request(wh.Data);
            }
        }
    }
}
