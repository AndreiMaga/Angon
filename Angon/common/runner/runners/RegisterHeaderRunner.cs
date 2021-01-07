using Angon.common.comprotocols.requests;
using Angon.common.headers;
using Angon.common.sender;
using Angon.common.storage;
using Angon.common.storage.data;
using Angon.common.utils;
using System;

namespace Angon.common.runner.runners
{
    class RegisterHeaderRunner
    {
        public static void Run(RequestWithHeader<RegisterHeader> rh)
        {
            string token = "";
            if (StorageProvider.GetInstance().SlaveExists(rh.header.Ip) == "")
            {
                token = Guid.NewGuid().ToString();
                Slave slave = new Slave(rh.header.Ip, rh.header.Port, 0, token);
                StorageProvider.GetInstance().RegisterSlave(slave);
            }

            RegisterResponseHeader rrh = new RegisterResponseHeader
            {
                Token = token
            };

            WraperHeader wh = new WraperHeader()
            {
                Data = ByteArrayUtils.ToByteArray(rrh),
                Type = HeaderTypes.RegisterResponseHeader
            };

            Sender.Send(wh, rh.Client);

        }
    }
}
