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
            if(!File.Exists($"{configFolder}/{configFile}"))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText($"{configFolder}/{configFile}", json);
            }
            else
            {
                string json = File.ReadAllText($"{configFolder}/{configFile}");
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }

        public static BotConfig bot;

        private const string configFolder = "Resources";
        private const string configFile = "config.json";
    }

    public class BotConfig
    {
        public string authKey = "";
        public string commandPrefix = "!";
        public string botName = "GarlicBot";
        public string botIconURL = "https://i.imgur.com/NFS0WeC.jpg";
        public LogSeverity logLevel = LogSeverity.Verbose;
        public string embedColor = "140,60,60";
        public long maxFileSize = 5000;
        public long progressUpdateDelay = 500;
    }
}
