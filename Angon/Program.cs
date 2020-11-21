using Angon.common.config;
using Angon.common.storage;
using Angon.master.server;

namespace Angon
{
    class Program
    {
        static void Main(string[] args)
        {

            //Force load Storage
            StorageProvider.GetInstance();

            if(ConfigReader.GetInstance().Config.Type == 0) // Master
            {
                // start new server
                new Server();
            }
            else if(ConfigReader.GetInstance().Config.Type == 1)
            {
                // TODO client
            }
            else
            {
                // Log that the type is not ok!
            }
        }
    }
}
