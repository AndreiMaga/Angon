using Angon.common.config;
using Angon.common.reciever;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Angon.master.server
{
    /// <summary>
    /// Server that runs on the master.
    /// </summary>
    class Server
    {
        /// <summary>
        /// The <see cref="TcpListener"/> for the server.
        /// </summary>
        readonly TcpListener ListeningServer = null;

        /// <summary>
        /// Default constructor that also starts the listening process.
        /// </summary>
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
            catch (Exception)
            {

            }
        }
    }
}
