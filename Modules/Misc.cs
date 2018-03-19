using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace GarlicBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        public async Task Echo([Remainder]string message)
        {
            IDisposable dispose = Context.Channel.EnterTypingState();
            await Context.Channel.SendMessageAsync(message);
            dispose.Dispose();
        }
    }
}
