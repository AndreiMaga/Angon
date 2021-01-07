using Angon.common.config;
using Angon.common.headers;
using Angon.common.reciever;
using Angon.common.sender;
using Angon.common.storage;
using Angon.common.utils;
using System.Net.Sockets;

namespace Angon.slave
{
    class Register
    {

        public static void RegisterToMaster()
        {

            if (StorageProvider.GetInstance().GetTokenAsSlave() != "")
                return;

            RegisterHeader rh = new RegisterHeader
            {
                Ip = ConfigReader.GetInstance().Config.PredefinedIP,
                Port = ConfigReader.GetInstance().Config.Port
            };

            WraperHeader wraperHeader = new WraperHeader
            {
                Data = ByteArrayUtils.ToByteArray(rh),
                Type = HeaderTypes.RegisterHeader
            };

            TcpClient client = new TcpClient(ConfigReader.GetInstance().Config.Ip, ConfigReader.GetInstance().Config.MastersPort);

            Sender.Send(wraperHeader, client);

            new Reciever().ProcessClient(client);
        }


    }
}
