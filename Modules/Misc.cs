using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

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
            await Utilities.SendMessage(
                message, // message body
                "Echo:", // message title
                Context); // command context
        }

        [Command("echo")]
        public async Task Echo_NoArgs()
        {
            await Utilities.SendMessage(
                String.Format(await Utilities.GetAlert("commandTooFewArgs"), "echo"), // message body
                await Utilities.GetAlert("commandErrorTitle"), // message title
                Context); // command context
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
            int result = (r.Next() % sides) + 1;

            await Utilities.SendMessage(
                String.Format(await Utilities.GetAlert("rollResult"), pre, result), // message body
                String.Format(await Utilities.GetAlert("rollOutput"), sides), // message title
                Context); // command context
        }

        [Command("roll")]
        public async Task Roll_NoArgs()
        {
            Random r = new Random();
            int sides = 6;
            int result = (r.Next() % sides) + 1;
            await Utilities.SendMessage(
                String.Format(await Utilities.GetAlert("rollResult"), "", result), // message body
                String.Format(await Utilities.GetAlert("rollOutput"), sides), // message title
                Context); // command context
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
            SocketUser user = Context.User;
            ulong id = user.Id;
            if(await PermissionsManager.GetPerm(id, Permissions.Restart)) {
                await Utilities.SendMessage(
                    String.Format(await Utilities.GetAlert("restartMessage"), Config.bot.botName), // message body
                    "Restarting:", // message title
                    Context); // command context
                Program.Restart();
            }
            else {
                await Utilities.SendMessage(
                    await Utilities.GetAlert("invalidPerms"), // message body
                    await Utilities.GetAlert("commandErrorTitle"), // message title
                    Context); // command context
            }
        }

        [Command("shutdown")]
        public async Task Shutdown() {
            SocketUser user = Context.User;
            ulong id = user.Id;
            if (await PermissionsManager.GetPerm(id, Permissions.Shutdown)) {
                await Utilities.SendMessage(
                    String.Format(await Utilities.GetAlert("shutdownMessage"), Config.bot.botName), // message body
                    "Restarting:", // message title
                    Context); // command context
                Program.Shutdown();
            }
            else {
                await Utilities.SendMessage(
                    await Utilities.GetAlert("invalidPerms"), // message body
                    await Utilities.GetAlert("commandErrorTitle"), // message title
                    Context); // command context
            }
        }
    }
}
