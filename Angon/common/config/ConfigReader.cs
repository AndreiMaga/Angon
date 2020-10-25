using System;
using System.IO;
using System.Text.Json;

namespace Angon.common.config
{
    /// <summary>
    /// SINGLETON
    /// Access the instance by calling getInstance()
    /// </summary>
    class ConfigReader
    {
        private static ConfigReader instance;
        public Config Config { get; private set; }
        private ConfigReader()
        {
            Config = JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"));
        }

        public static ConfigReader GetInstance()
        {
            if(instance == null)
            {
                instance = new ConfigReader();
            }
            return instance;
        }

    }
}
