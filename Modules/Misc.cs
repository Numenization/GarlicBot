using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace GarlicBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help()
        {
            await Context.Channel.SendMessageAsync("", false, await HelpManager.BuildEmbed());
        }

        [Command("echo")]
        public async Task Echo([Remainder]string message)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Echo:");
            embed.WithDescription(message);
            embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
            embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("echo")]
        public async Task Echo_NoArgs()
        {
            var embed = new EmbedBuilder();
            embed.WithTitle(await Utilities.GetAlert("commandErrorTitle"));
            embed.WithDescription(String.Format(await Utilities.GetAlert("commandTooFewArgs"), "echo"));
            embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
            embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("roll")]
        public async Task Roll([Remainder]string message)
        {
            int sides = 6;
            string pre = "";
            Random r = new Random();
            try
            {
                sides = int.Parse(message);
            }
            catch
            {
                pre = await Utilities.GetAlert("rollCommandCouldntParse");
            }
            var embed = new EmbedBuilder();
            embed.WithTitle(String.Format(await Utilities.GetAlert("rollOutput"), sides));
            int result = (r.Next() % sides) + 1;
            embed.WithDescription(String.Format(await Utilities.GetAlert("rollResult"), pre, result));
            embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
            embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("roll")]
        public async Task Roll_NoArgs()
        {
            Random r = new Random();
            int sides = 6;
            var embed = new EmbedBuilder();
            embed.WithTitle(String.Format(await Utilities.GetAlert("rollOutput"), sides));
            int result = (r.Next() % sides) + 1;
            embed.WithDescription(String.Format(await Utilities.GetAlert("rollResult"), "", result));
            embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
            embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("github")]
        public async Task Git()
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("GarlicBot GitHub Link");
            embed.WithUrl("https://github.com/Numenization/GarlicBot");
            embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
            embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("restart")]
        public async Task Restart() {
            if((Context.User as IGuildUser).GuildPermissions.Administrator) {
                var embed = new EmbedBuilder();
                embed.WithDescription(String.Format(await Utilities.GetAlert("restartMessage"), Config.bot.botName));
                embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
                embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
                await Context.Channel.SendMessageAsync("", false, embed.Build());
                Program.Restart();
            }
            else {
                var embed = new EmbedBuilder();
                embed.WithTitle(await Utilities.GetAlert("commandErrorTitle"));
                embed.WithDescription(await Utilities.GetAlert("invalidPerms"));
                embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
                embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("shutdown")]
        public async Task Shutdown() {
            if ((Context.User as IGuildUser).GuildPermissions.Administrator) {
                var embed = new EmbedBuilder();
                embed.WithDescription(String.Format(await Utilities.GetAlert("shutdownMessage"), Config.bot.botName));
                embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
                embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
                await Context.Channel.SendMessageAsync("", false, embed.Build());
                Program.Shutdown();
            }
            else {
                var embed = new EmbedBuilder();
                embed.WithTitle(await Utilities.GetAlert("commandErrorTitle"));
                embed.WithDescription(await Utilities.GetAlert("invalidPerms"));
                embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
                embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }
    }
}
