using Angon.common.comprotocols.requests;
using Angon.common.config;
using Angon.common.headers;
using Angon.common.sender;
using Angon.common.storage;
using Angon.common.utils;

namespace Angon.common.runner.runners
{
    class ServerAvailableRunner
    {
        public static void Run(RequestWithHeader<ServerAvailableHeader> sah)
        {
            if (ConfigReader.GetInstance().Config.Type == 0)
            {

            }
            else
            {
                RunAsSlave(sah);
            }

        }
        public static void RunAsMaster(RequestWithHeader<ServerAvailableHeader> sah)
        {
            if (sah.header.UniqueToken != "")
            {
                StorageProvider.GetInstance().UpdateSlave(sah.header.UniqueToken, sah.header.Available);

            }
        }
        public static void RunAsSlave(RequestWithHeader<ServerAvailableHeader> sah)
        {
            ServerAvailableHeader sahresp = new ServerAvailableHeader()
            {
                Available = StorageProvider.GetInstance().NumberOfJobsToBeDone() == 0 ? true : false,
                UniqueToken = StorageProvider.GetInstance().GetTokenAsSlave()
            };
            WraperHeader wraper = new WraperHeader()
            {
                Type = HeaderTypes.ServerAvailableHeader,
                Data = ByteArrayUtils.ToByteArray(sahresp)
            };
            Sender.Send(wraper, sah.Client);
        }
    }
}
