using System.IO;
using Discord;
using Newtonsoft.Json;

namespace GarlicBot
{
    class Config
    {
        static Config()
        {
            if(!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }
            if(!File.Exists(configFolder + "/" + configFile))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }

        public static BotConfig bot;

        private const string configFolder = "Resources";
        private const string configFile = "config.json";
    }

    public struct BotConfig
    {
        public string authKey;
        public string commandPrefix;
        public LogSeverity logLevel;
        public string embedColor;
    }
}
