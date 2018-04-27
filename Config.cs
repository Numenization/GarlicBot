using System.Collections.Generic;
using System.IO;
using Discord;
using Newtonsoft.Json;

namespace GarlicBot
{
    class Config
    {
        /// <summary>
        /// Static class storing bot configuration information
        /// </summary>
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

        /// <summary>
        /// The current instance of bot configuration
        /// </summary>
        public static BotConfig bot;

        private const string configFolder = "Resources";
        private const string configFile = "config.json";
    }

    public class BotConfig
    {
        /// <summary>
        /// Authentication key
        /// </summary>
        public string authKey = "";
        /// <summary>
        /// Prefix for commands
        /// </summary>
        public string commandPrefix = "!";
        /// <summary>
        /// Name of the bot as it appears in the console and Discord chat
        /// </summary>
        public string botName = "GarlicBot";
        /// <summary>
        /// Image link to bot icon as it appears in Discord chat
        /// </summary>
        public string botIconURL = "https://i.imgur.com/NFS0WeC.jpg";
        /// <summary>
        /// Logging level for console
        /// </summary>
        public LogSeverity logLevel = LogSeverity.Verbose;
        /// <summary>
        /// Color for the side strip of Discord embeds
        /// </summary>
        public string embedColor = "140,60,60";
        /// <summary>
        /// Max download size for images, in KB (Discord only allows 5MB)
        /// </summary>
        public long maxFileSize = 5000;
        /// <summary>
        /// Number of milliseconds between progress updates
        /// </summary>
        public long progressUpdateDelay = 500;
        /// <summary>
        /// List of IDs for all the specified bot administrators
        /// </summary>
        public List<ulong> adminUsers = new List<ulong>();
    }
}
