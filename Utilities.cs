using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;

namespace GarlicBot
{
    class Utilities
    {
        static Utilities()
        {
            try
            {
                string json = File.ReadAllText("SystemLang/alerts.json");
                var data = JsonConvert.DeserializeObject<dynamic>(json);
                _alerts = data.ToObject<Dictionary<string,string>>();
            }
            catch(FileNotFoundException e)
            {
                // log an error
            }
        }

        public static async Task<string> GetAlert(string key)
        {
            if(_alerts.ContainsKey(key))
            {
                return _alerts[key];
            }
            else
            {
                await Log("Could not find alert \"" + key + "\" in json!", LogSeverity.Error);
                return "";
            }
        }

        public static async Task Log(string msg, LogSeverity level)
        {
            if(level <= Config.bot.logLevel)
            {
                Console.WriteLine("[GarlicBot] " + msg);
            }
        }

        private static Dictionary<string, string> _alerts;
    }
}
