using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;

namespace GarlicBot.Modules
{
    /// <summary>
    /// Class used to help manage information for Help commands
    /// </summary>
    class HelpManager
    {
        static HelpManager()
        {
            try
            {
                string json = File.ReadAllText(helpFilePath);
                helpNodes = JsonConvert.DeserializeObject<List<HelpNode>>(json);
            }
            catch(Exception e)
            {
                Utilities.Log($"{e.HResult} {helpFilePath}", LogSeverity.Critical);
            }
        }

        /// <summary>
        /// Gets an embed with information about all the available commands
        /// </summary>
        /// <returns>The embed with help information</returns>
        public static async Task<Embed> BuildEmbed()
        {
            var embed = new EmbedBuilder();
            embed.WithTitle(await Utilities.GetAlert("helpCommand"));
            foreach(HelpNode node in helpNodes)
            {
                string value = "";
                foreach(string s in node.lines)
                {
                    value += $"{s}\n";
                }
                embed.AddField(new EmbedFieldBuilder
                {
                    Name = node.commandName,
                    Value = value
                });
            }
            embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
            embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);

            return embed.Build();
        }

        private static string helpFilePath = "SystemLang/helpdata.json";
        private static List<HelpNode> helpNodes;
    }

    class HelpNode
    {
        public string commandName = "null";
        public List<string> lines = new List<string>();
    }
}
