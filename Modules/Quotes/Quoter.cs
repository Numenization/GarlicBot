using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace GarlicBot.Modules.Quotes
{
    public class Quoter : ModuleBase<SocketCommandContext>
    {
        [Command("addquote")]
        public async Task AddQuote(params string[] args)
        {
            if(args.Length < 2)
            {
                return;
            }
            string remainder = "";
            for(int i = 1; i < args.Length; i++)
            {
                remainder += $"{args[i]} ";
            }
            DateTime now = DateTime.Now;
            Quote quote = new Quote
            {
                author = args[0],
                text = remainder,
                date = $"{now.Month}/{now.Day}/{now.Year} [{now.Hour}:{now.Minute}]"
            };
            QuoteManager.AddQuote(quote);
            var embed = new EmbedBuilder();
            embed.WithTitle(await Utilities.GetAlert("quoteAdded"));
            embed.WithDescription(String.Format(await Utilities.GetAlert("quoteAddedBody"), quote.author));
            embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
            embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("quote")]
        public async Task Quote()
        {
            // we get the quote from the last message said by another person
            //IDisposable dispose2 = Context.Channel.EnterTypingState();
            var messages = Context.Channel.GetMessagesAsync(5).Flatten();
            var enumerator = messages.GetEnumerator();
            await enumerator.MoveNext();
            await enumerator.MoveNext();
            IMessage quotedMessage = enumerator.Current;
            DateTime now = quotedMessage.Timestamp.LocalDateTime;
            Quote quote = new Quote
            {
                author = quotedMessage.Author.Username,
                text = quotedMessage.Content,
                date = $"{now.Month}/{now.Day}/{now.Year} [{now.Hour}:{now.Minute}]"
            };
            QuoteManager.AddQuote(quote);
            var embed = new EmbedBuilder();
            embed.WithTitle(await Utilities.GetAlert("quoteAdded"));
            embed.WithDescription(String.Format(await Utilities.GetAlert("quoteAddedBody"), quote.author));
            embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
            embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
            await Context.Channel.SendMessageAsync("", false, embed.Build());
            //dispose2.Dispose();
        }

        [Command("getquote")]
        public async Task GetQuoteRandom()
        {
            Quote quote = QuoteManager.GetRandomQuote();
            var embed = new EmbedBuilder();
            embed.WithTitle(quote.author);
            embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
            embed.WithDescription(quote.text);
            embed.WithFooter(quote.date);
            embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
