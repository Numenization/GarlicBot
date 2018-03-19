using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace GarlicBot
{
    public class CommandHandler
    {
        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            _service.AddModulesAsync(Assembly.GetEntryAssembly());

            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            SocketUserMessage msg = s as SocketUserMessage;
            if(msg != null)
            {
                SocketCommandContext context = new SocketCommandContext(_client, msg);
                await Utilities.Log(context.User.Username + ": " + context.Message.Content, LogSeverity.Verbose);
                int argPos = 0;
                if (msg.HasStringPrefix(Config.bot.commandPrefix, ref argPos)
                    || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
                {
                    var result = await _service.ExecuteAsync(context, argPos);
                    if(!result.IsSuccess)
                    {
                        await context.Channel.SendMessageAsync(await Utilities.GetAlert("commandNotFound"));
                    }
                }
            }
        }

        private DiscordSocketClient _client;
        private CommandService _service;
    }
}
