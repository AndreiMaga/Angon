using Newtonsoft.Json;
using Serilog;
using System.IO;

namespace Angon.common.config
{
    /// <summary>
    /// SINGLETON
    /// Access the instance by calling getInstance()
    /// </summary>
    class ConfigReader
    {
        /// <summary>
        /// Singleton instance 
        /// </summary>
        private static ConfigReader instance;

        /// <summary>
        /// The loaded config
        /// </summary>
        public Config Config { get; private set; }

        /// <summary>
        /// Signleton constructor
        /// </summary>
        private ConfigReader()
        {
            Log.Information("Loading config.json");
            Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));
        }

        /// <summary>
        /// Signleton getter
        /// </summary>
        /// <returns>Signleton <see cref="instance"/></returns>
        public static ConfigReader GetInstance()
        {
            if (instance == null)
            {
                instance = new ConfigReader();
            }
            return instance;
        }

    }
}
