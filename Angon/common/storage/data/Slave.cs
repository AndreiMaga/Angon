namespace Angon.common.storage.data
{
    class Slave
    {


        public Slave(string ip, int port, int availableForWork, string uniquetoken)
        {
            Ip = ip;
            Port = port;
            AvailableForWork = availableForWork;
            UniqueToken = uniquetoken;
        }

        public string Ip { get; set; }
        public int Port { get; set; }
        public int AvailableForWork { get; set; }
        public string UniqueToken { get; set; }

        public bool HasJob { get; set; } = false;
        public string AssignedJob { get; set; } = "";
    }
}
