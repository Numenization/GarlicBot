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
                await Log($"Could not find alert \"{key}\" in json!", LogSeverity.Error);
                return "";
            }
        }

        public static async Task Log(string msg, LogSeverity level)
        {
            if(level <= Config.bot.logLevel)
            {
                Console.WriteLine($"[GarlicBot] {msg}");
                await WriteToLogFile(msg, "GarlicBot");
            }
        }

        public static async Task<Color> ParseColor(string rgb)
        {
            int r;
            int g;
            int b;
            string[] arr = rgb.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in arr)
            {
                s.Trim();
            }
            if(arr.Length == 3)
            {
                try
                {
                    r = int.Parse(arr[0]);
                    g = int.Parse(arr[1]);
                    b = int.Parse(arr[2]);
                }
                catch(Exception e)
                {
                    await Log($"Error trying to parse color!\n{e.Message}", LogSeverity.Error);
                    return new Color(255,255,255);
                }

                return new Color(r, g, b);
            }
            return new Color(255, 255, 255);
        }

        public static async Task WriteToLogFile(string msg)
        {
            if(logFile == null)
            {
                if(!Directory.Exists("Logs"))
                {
                    Directory.CreateDirectory("Logs");
                }
                logFile = new StreamWriter($"Logs/{DateTime.UtcNow.ToFileTime()}_Log.txt", true);
                logFile.AutoFlush = true;
            }
            await logFile.WriteLineAsync($"[GarlicBot] {msg}");
        }

        public static async Task WriteToLogFile(string msg, string source)
        {
            if (logFile == null)
            {
                if (!Directory.Exists("Logs"))
                {
                    Directory.CreateDirectory("Logs");
                }
                logFile = new StreamWriter($"Logs/{DateTime.UtcNow.ToFileTime()}_Log.txt", true);
                logFile.AutoFlush = true;
            }
            await logFile.WriteLineAsync($"[{source}] {msg}");
        }

        public static async Task WriteToLogFile(LogMessage logMessage)
        {
            if (logFile == null)
            {
                if (!Directory.Exists("Logs"))
                {
                    Directory.CreateDirectory("Logs");
                }
                logFile = new StreamWriter($"Logs/{DateTime.UtcNow.ToFileTime()}_Log.txt", true);
                logFile.AutoFlush = true;
            }
            await logFile.WriteLineAsync($"[{logMessage.Source}] {logMessage.Message}");
        }

        public static void CloseLogFile()
        {
            if (logFile == null) return;
            logFile.Close();
        }

        private static StreamWriter logFile = null;
        private static Dictionary<string, string> _alerts;
    }
}
