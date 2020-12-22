using System.Collections.Generic;
using System.IO;

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
        public int Type { get; set; } = 0;

        /// <summary>
        /// The IP of the master
        /// </summary>
        public string Ip { get; set; } = "127.0.0.1";

        /// <summary>
        /// The port of the master
        /// </summary>
        public int Port { get; set; } = 8989;

        /// <summary>
        /// Path to save temporary and result files to
        /// </summary>
        public string SavePath { get; set; } = Path.GetTempPath();

        /// <summary>
        /// The maximum amount of read size in memory
        /// </summary>
        public int ReadSize { get; set; } = 512;

        /// <summary>
        /// The maximum amount of write size in memory
        /// </summary>
        public int WriteSize { get; set; } = 512;

        /// <summary>
        /// TODO this part
        /// </summary>
        public string PredefinedIP { get; set; } = "";

        /// <summary>
        /// The version of the program
        /// </summary>
        public string Version { get; set; } = "0.0.1";

        public int MilisecondsToSleep { get; set; } = 500;

        public bool DisableExternalSplitter { get; set; } = true;

        public bool VerifySignature { get; set; } = true;

        public bool DeleteAfterUnzip { get; set; } = true;

        public bool DeleteSplitFolderAfterJobZip { get; set; } = true;

        public bool DeleteInputFolderAfterSplit { get; set; } = true;

        public bool RestrictUnknownConnections { get; set; } = false;

        public List<string> KnownIPs { get; set; } = new List<string>();
    }
}
