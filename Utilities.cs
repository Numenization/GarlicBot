using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
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
                Log($"{e.HResult} SystemLang/alerts.json", LogSeverity.Error);
            }
        }

        /// <summary>
        /// Method that gets an alert from alerts.json given a key
        /// </summary>
        /// <param name="key">Key to get alert for</param>
        /// <returns></returns>
        public static async Task<string> GetAlert(string key)
        {
            if(_alerts.ContainsKey(key))
            {
                return _alerts[key];
            }
            else
            {
                await LogAsync($"Could not find alert \"{key}\" in json!", LogSeverity.Error);
                return "";
            }
        }

        public static async Task SendMessage(string message, string title, SocketCommandContext context) {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithDescription(message);
            embed.WithTitle(title);
            embed.WithColor(await ParseColor(Config.bot.embedColor));
            embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
            await context.Channel.SendMessageAsync("", false, embed.Build());
        }

        /// <summary>
        /// Synchronously log a message to the console
        /// </summary>
        /// <param name="msg">Message to log</param>
        /// <param name="level">Logging level</param>
        public static void Log(string msg, LogSeverity level) {
            if (level <= Config.bot.logLevel) {
                Console.WriteLine($"[GarlicBot] {msg}");
                WriteToLogFile(msg, "GarlicBot");
            }
        }

        /// <summary>
        /// Asynchronously log a message to the console
        /// </summary>
        /// <param name="msg">Message to log</param>
        /// <param name="level">Logging level</param>
        /// <returns></returns>
        public static async Task LogAsync(string msg, LogSeverity level)
        {
            if(level <= Config.bot.logLevel)
            {
                Console.WriteLine($"[GarlicBot] {msg}");
                await WriteToLogFileAsync(msg, "GarlicBot");
            }
        }

        /// <summary>
        /// Parses a color from a string in the form of "R,G,B"
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns>The color represented by the string, if it exists</returns>
        public static async Task<Color> ParseColor(string rgb)
        {
            int r, g, b;
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
                    await LogAsync($"Error trying to parse color!\n{e.Message}", LogSeverity.Error);
                    return new Color(255,255,255);
                }

                return new Color(r, g, b);
            }
            return new Color(255, 255, 255);
        }

        /// <summary>
        /// Asynchronously write a string to the log file
        /// </summary>
        /// <param name="msg">Message to log</param>
        /// <returns></returns>
        public static async Task WriteToLogFileAsync(string msg)
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

        /// <summary>
        /// Synchronously write a string to the log file
        /// </summary>
        /// <param name="msg">Message to write</param>
        public static void WriteToLogFile(string msg)
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
            logFile.WriteLine($"[GarlicBot] {msg}");
        }

        /// <summary>
        /// Synchronously write a string to the log file
        /// </summary>
        /// <param name="msg">Message to write</param>
        /// <param name="source">Source of call</param>
        public static void WriteToLogFile(string msg, string source) {
            if (logFile == null) {
                if (!Directory.Exists("Logs")) {
                    Directory.CreateDirectory("Logs");
                }
                logFile = new StreamWriter($"Logs/{DateTime.UtcNow.ToFileTime()}_Log.txt", true);
                logFile.AutoFlush = true;
            }
            logFile.WriteLine($"[{source}] {msg}");
        }

        /// <summary>
        /// Asynchronously write a string to the log file
        /// </summary>
        /// <param name="msg">Message to write</param>
        /// <param name="source">Source of call</param>
        public static async Task WriteToLogFileAsync(string msg, string source)
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

        /// <summary>
        /// Asynchronously write a string to the log file
        /// </summary>
        /// <param name="logMessage">Object containing message to write and source information</param>
        /// <returns></returns>
        public static async Task WriteToLogFileAsync(LogMessage logMessage)
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
