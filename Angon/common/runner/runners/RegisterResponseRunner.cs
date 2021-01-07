using Angon.common.comprotocols.requests;
using Angon.common.headers;
using Angon.common.storage;
using Angon.common.storage.data;

namespace Angon.common.runner.runners
{
    class RegisterResponseRunner
    {
        public static void Run(RequestWithHeader<RegisterResponseHeader> rrh)
        {
            Slave slave = new Slave("", 0, 1, rrh.header.Token);
            StorageProvider.GetInstance().RegisterSlave(slave);
        }
    }
}
