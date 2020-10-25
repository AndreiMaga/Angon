using Angon.common.config;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Angon.master.server
{
    class Server
    {
        TcpListener ListeningServer = null;
        Server()
        {
            try
            {
                ListeningServer = new TcpListener(
                    IPAddress.Parse(ConfigReader.GetInstance().Config.Ip),
                    ConfigReader.GetInstance().Config.Port);
                ListeningServer.Start();
                while (true)
                {
                    Task task = new Task(() =>
                    {
                        TcpClient client = ListeningServer.AcceptTcpClient();
                        new Reciever().ProcessClient(client);
                    });
                    task.Start();
                }
            }
            catch (SocketException e)
            {

            }
        }
    }
}
