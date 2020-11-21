namespace Angon.common.config
{
    public class Config
    {
        public int Type { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public string SavePath { get; set; }

        public int ReadSize { get; set; }

        public int WriteSize { get; set; }

        public string PredefinedIP { get; set; }

        public string Version { get; set; }
    }
}
