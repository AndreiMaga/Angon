namespace Angon.common.config
{
    /// <summary>
    /// C# class representing <see cref="config.json"/>
    /// </summary>
    public class Config
    {
        /// <summary>
        /// The type of the local configuration
        /// 0 : Master
        /// 1 : Slave
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// The IP of the master
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// The port of the master
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Path to save temporary and result files to
        /// </summary>
        public string SavePath { get; set; }

        /// <summary>
        /// The maximum amount of read size in memory
        /// </summary>
        public int ReadSize { get; set; }

        /// <summary>
        /// The maximum amount of write size in memory
        /// </summary>
        public int WriteSize { get; set; }

        /// <summary>
        /// TODO this part
        /// </summary>
        public string PredefinedIP { get; set; }


        /// <summary>
        /// The version of the program
        /// </summary>
        public string Version { get; set; }
    }
}
