using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace GarlicBot.Modules.ImageProcessing
{
    public class ImageCommands : ModuleBase<SocketCommandContext> {
        [Command("read", RunMode = RunMode.Async)]
        public async Task readImage() {
            var attachments = Context.Message.Attachments.GetEnumerator();
            if(attachments.MoveNext()) {
                string url = attachments.Current.Url;
                ImageReader reader = new ImageReader();
                var progress = new Progress<string>();
                progress.ProgressChanged += (s, e) => {
                    Utilities.Log(e, LogSeverity.Verbose);
                };
                if (await reader.ReadFromUrl(url, progress)) {
                    //do stuff with image
                }
            }
        }

        [Command("read", RunMode = RunMode.Async)]
        public async Task readImageUrl([Remainder]string url) {
            ImageReader reader = new ImageReader();
            var progress = new Progress<string>();
            progress.ProgressChanged += (s, e) => {
                Utilities.Log(e, LogSeverity.Verbose);
            };
            if (await reader.ReadFromUrl(url, progress)) {
                // do stuff with image
            }
        }
    }
}
