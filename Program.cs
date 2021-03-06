﻿using System;
using System.IO;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using GarlicBot.Modules;
using GarlicBot.Modules.Quotes;
using GarlicBot.Modules.ImageProcessing;

namespace GarlicBot
{
    public class Program {
        static void Main(string[] args)
        => (program = new Program()).StartAsync().GetAwaiter().GetResult();


        public async Task StartAsync() {
            if (!Directory.Exists("Resources")) {
                await Utilities.LogAsync(await Utilities.GetAlert("firstTimeRunning"), LogSeverity.Error);
            }
            if (Config.bot.authKey == "" || Config.bot.authKey == null) {
                await Utilities.LogAsync(await Utilities.GetAlert("missingAuthKey"), LogSeverity.Error);
                Console.ReadKey();
                Environment.Exit(0);
            }
            _client = new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = Config.bot.logLevel
            });
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, Config.bot.authKey);
            await _client.StartAsync();
            Client = _client;

            _handler = new CommandHandler(_client);

            _client.Ready += async () => {
                // Bot is ready, initialize console command input
                await Utilities.LogAsync("Bot is connected and running. Type \"help\" to see available console commands.", LogSeverity.Info);
            };

            bool running = true;
            while (running) {
                string input = Console.ReadLine();
                if (input.ToLower() == "restart") {
                    await RestartAsync();
                    running = false;
                }
                else if (input.ToLower() == "exit" || input.ToLower() == "close") {
                    Shutdown();
                }
                else if (input.ToLower() == "help") {
                    Console.WriteLine("[Console] GarlicBot Console Commands:");
                    Console.WriteLine(" - restart : Refreshes the bot");
                    Console.WriteLine(" - exit    : Safely logs out and closes the application");
                    Console.WriteLine(" - help    : Shows this text");
                }
                else if (input.ToLower() == "showquotes") {
                    foreach (Quote q in QuoteManager.quotes) {
                        Console.WriteLine($"Author: {q.author}\nText: {q.text}\nDate:{q.date}");
                    }
                }
                else {
                    Console.WriteLine($"[Console] Invalid command \"{input}\"");
                }
            }

            await Task.Delay(-1);
        }

        private async Task Log(LogMessage msg) {
            Console.WriteLine($"[{msg.Source}] {msg.Message}");
            await Utilities.WriteToLogFileAsync(msg);
        }

        /// <summary>
        /// Restarts the program
        /// </summary>
        /// <returns></returns>
        public static async Task RestartAsync() {
            await Client.LogoutAsync();
            await Utilities.LogAsync("Restarting...", LogSeverity.Info);
            program.StartAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Restarts the program
        /// </summary>
        public static void Restart() {
            Client.LogoutAsync();
            Utilities.Log("Restarting...", LogSeverity.Info);
            program.StartAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Ends the program
        /// </summary>
        public static void Shutdown() {
            Console.WriteLine("[Console] Closing GarlicBot...");
            Environment.Exit(0);
        }

        // TODO: Bot does not log out when application is closed
        // fix this

        private static DiscordSocketClient Client;
        private static Program program;

        private CommandHandler _handler;
        private DiscordSocketClient _client;
    }
}
