using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace GarlicBot.Modules
{
    public class Quoter : ModuleBase<SocketCommandContext>
    {
        [Command("addquote")]
        public async Task addQuote(params string[] args)
        {
            if(args.Length < 2)
            {
                IDisposable dispose = Context.Channel.EnterTypingState();
                var embed = new EmbedBuilder();
                embed.WithTitle(await Utilities.GetAlert("commandErrorTitle"));
                embed.WithDescription(String.Format(await Utilities.GetAlert("commandTooFewArgs"), "echo"));
                embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
                embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
                await Context.Channel.SendMessageAsync("", false, embed.Build());
                dispose.Dispose();
                return;
            }
            string remainder = "";
            for(int i = 1; i < args.Length; i++)
            {
                remainder += $"{args[i]} ";
            }
            DateTime now = DateTime.Now;
            QuoteManager.AddQuote(new Quote
            {
                author = args[0],
                text = remainder,
                date = $"{now.Month}/{now.Day}/{now.Year} [{now.Hour}:{now.Minute}]"
            });

        }
    }
}
