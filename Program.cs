using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace GarlicBot
{
    public class Program
    {
        static void Main(string[] args) 
        => (program = new Program()).StartAsync().GetAwaiter().GetResult();


        public async Task StartAsync()
        { 
            if(Config.bot.authKey == "" || Config.bot.authKey == null)
            {
                await Utilities.Log(await Utilities.GetAlert("missingAuthKey"), LogSeverity.Error);
                return;
            }
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Config.bot.logLevel
            });
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, Config.bot.authKey);
            await _client.StartAsync();
            await Utilities.Log("Bot is connected and running. Type \"help\" to see available console commands.", LogSeverity.Info);
            Client = _client;

            _handler = new CommandHandler(_client);

            bool running = true;
            while(running)
            {
                string input = Console.ReadLine();
                if(input.ToLower() == "restart")
                {
                    await Restart();
                    running = false;
                }
                else if(input.ToLower() == "exit" || input.ToLower() == "close")
                {
                    Console.WriteLine("[Console] Closing GarlicBot...");
                    Environment.Exit(0);
                }
                else if(input.ToLower() == "help")
                {
                    Console.WriteLine("[Console] GarlicBot Console Commands:");
                    Console.WriteLine(" - restart : Refreshes the bot");
                    Console.WriteLine(" - exit    : Safely logs out and closes the application");
                    Console.WriteLine(" - help    : Shows this text");
                }
                else
                {
                    Console.WriteLine($"[Console] Invalid command \"{input}\"");
                }
            }

            await Task.Delay(-1);
        }

        private async Task Log(LogMessage msg)
        {
            Console.WriteLine($"[{msg.Source}] {msg.Message}");
            await Utilities.WriteToLogFile(msg);
        }

        public static async Task Restart()
        {
            await Client.LogoutAsync();
            await Utilities.Log("Restarting...", LogSeverity.Info);
            program.StartAsync().GetAwaiter().GetResult();
        }

        // TODO: Bot does not log out when application is closed
        // fix this

        private static DiscordSocketClient Client;
        private static Program program;

        private CommandHandler _handler;
        private DiscordSocketClient _client;
    }
}
