using Angon.common.config;
using Angon.common.reciever;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Angon.master.server
{
    class Server
    {
        readonly TcpListener ListeningServer = null;
        public Server()
        {
            try
            {
                ListeningServer = new TcpListener(
                    IPAddress.Parse(ConfigReader.GetInstance().Config.Ip),
                    ConfigReader.GetInstance().Config.Port);
                ListeningServer.Start();
                while (true)
                {
                    TcpClient client = ListeningServer.AcceptTcpClient();
                    Task task = new Task(() =>
                    {
                        new Reciever().ProcessClient(client);
                    });
                    task.Start();
                }
            }
            catch (SocketException)
            {

            }
        }
    }
}
