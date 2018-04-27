using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace GarlicBot.Modules.ImageProcessing
{
    public class ImageCommands : ModuleBase<SocketCommandContext> {
        [Command("scramble", RunMode = RunMode.Async)]
        public async Task ScrambleAttachment() {
            SocketUser user = Context.User;
            ulong id = user.Id;
            if (!PermissionsManager.GetPerm(id, Permissions.ProcessImage)) {
                var embed = new EmbedBuilder();
                embed.WithTitle(await Utilities.GetAlert("commandErrorTitle"));
                embed.WithDescription(await Utilities.GetAlert("invalidPerms"));
                embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
                embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
                await Context.Channel.SendMessageAsync("", false, embed.Build());
                return;
            }

            var attachments = Context.Message.Attachments.GetEnumerator();
            if(attachments.MoveNext()) {
                string url = attachments.Current.Url;
                var reader = new ImageReader();
                var error = new ImageReadError();
                var progress = new Progress<string>();

                progress.ProgressChanged += (s, e) => {
                    Utilities.Log(e, LogSeverity.Verbose);
                };

                if (await reader.ReadFromUrl(url, progress, error)) {
                    //do stuff with image
                    ImageProcessor processor = new ImageProcessor(reader);
                    var processProgress = new Progress<double>();

                    processProgress.ProgressChanged += (s, e) => {
                        Utilities.Log($"Processing {e}%", LogSeverity.Verbose);
                    };

                    string file = await processor.Scramble(processProgress);
                    await Utilities.LogAsync("Sending file...", LogSeverity.Info);
                    await Context.Channel.SendFileAsync(file);
                }
                else {
                    var embed = new EmbedBuilder();
                    embed.WithTitle(await Utilities.GetAlert("commandErrorTitle"));
                    embed.WithDescription(error.ErrorReason);
                    embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
                    embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
            }
        }

        [Command("scramble", RunMode = RunMode.Async)]
        public async Task ScrambleURL([Remainder]string url) {
            SocketUser user = Context.User;
            ulong id = user.Id;
            if (!PermissionsManager.GetPerm(id, Permissions.ProcessImage)) {
                var embed = new EmbedBuilder();
                embed.WithTitle(await Utilities.GetAlert("commandErrorTitle"));
                embed.WithDescription(await Utilities.GetAlert("invalidPerms"));
                embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
                embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
                await Context.Channel.SendMessageAsync("", false, embed.Build());
                return;
            }

            var reader = new ImageReader();
            var error = new ImageReadError();
            var progress = new Progress<string>();

            progress.ProgressChanged += (s, e) => {
                Utilities.Log(e, LogSeverity.Verbose);
            };

            if (await reader.ReadFromUrl(url, progress, error)) {
                ImageProcessor processor = new ImageProcessor(reader);
                var processProgress = new Progress<double>();

                processProgress.ProgressChanged += (s, e) => {
                    Utilities.Log($"Processing {e}%", LogSeverity.Verbose);
                };

                string file = await processor.Scramble(processProgress);
                await Utilities.LogAsync("Sending file...", LogSeverity.Info);
                await Context.Channel.SendFileAsync(file);
            }
            else {
                var embed = new EmbedBuilder();
                embed.WithTitle(await Utilities.GetAlert("commandErrorTitle"));
                embed.WithDescription(error.ErrorReason);
                embed.WithColor(await Utilities.ParseColor(Config.bot.embedColor));
                embed.WithAuthor(Config.bot.botName, Config.bot.botIconURL);
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }
    }
}
