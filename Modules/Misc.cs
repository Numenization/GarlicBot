using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Audio;

namespace GarlicBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        public async Task Echo([Remainder]string message)
        {
            IDisposable dispose = Context.Channel.EnterTypingState();
            var embed = new EmbedBuilder();
            embed.WithTitle("Echo:");
            embed.WithDescription(message);
            embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
            await Context.Channel.SendMessageAsync("", false, embed.Build());
            dispose.Dispose();
        }

        [Command("echo")]
        public async Task Echo_NoArgs()
        {
            IDisposable dispose = Context.Channel.EnterTypingState();
            var embed = new EmbedBuilder();
            embed.WithTitle(await Utilities.GetAlert("commandErrorTitle"));
            embed.WithDescription(String.Format(await Utilities.GetAlert("commandTooFewArgs"), "echo"));
            embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
            await Context.Channel.SendMessageAsync("", false, embed.Build());
            dispose.Dispose();
        }

        [Command("roll")]
        public async Task Roll([Remainder]string message)
        {
            IDisposable dispose = Context.Channel.EnterTypingState();
            int sides = 6;
            string pre = "";
            try
            {
                sides = int.Parse(message);
            }
            catch (Exception e)
            {
                pre = await Utilities.GetAlert("rollCommandCouldntParse");
            }
            var embed = new EmbedBuilder();
            embed.WithTitle(String.Format(await Utilities.GetAlert("rollOutput"), sides));
            Random r = new Random();
            int result = (r.Next() % sides) + 1;
            embed.WithDescription(String.Format(await Utilities.GetAlert("rollResult"), pre, result));
            embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
            await Context.Channel.SendMessageAsync("", false, embed.Build());
            dispose.Dispose();
        }

        [Command("roll")]
        public async Task Roll_NoArgs()
        {
            IDisposable dispose = Context.Channel.EnterTypingState();
            int sides = 6;
            var embed = new EmbedBuilder();
            embed.WithTitle(String.Format(await Utilities.GetAlert("rollOutput"), sides));
            Random r = new Random();
            int result = (r.Next() % sides) + 1;
            embed.WithDescription(String.Format(await Utilities.GetAlert("rollResult"), "", result));
            embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
            await Context.Channel.SendMessageAsync("", false, embed.Build());
            dispose.Dispose();
        }
    }
}
