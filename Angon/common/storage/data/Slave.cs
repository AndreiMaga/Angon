namespace Angon.common.storage.data
{
    class Slave
    {
        string ip;
        int port;
        bool availableForWork;

        public Slave(string ip, int port, bool availableForWork)
        {
            Ip = ip;
            Port = port;
            AvailableForWork = availableForWork;
        }

        public string Ip { get => ip; set => ip = value; }
        public int Port { get => port; set => port = value; }
        public bool AvailableForWork { get => availableForWork; set => availableForWork = value; }
    }
}
