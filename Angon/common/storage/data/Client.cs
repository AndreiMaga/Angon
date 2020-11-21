
namespace Angon.common.storage.data
{
    class Client
    {
        public string Ip { get; set; }
        public string Port { get; set; }

        public Client(string ip, string port)
        {
            Ip = ip;
            Port = port;
        }

    }
}
