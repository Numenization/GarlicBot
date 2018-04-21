using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace GarlicBot.Modules.ImageProcessing
{
    public class ImageCommands : ModuleBase<SocketCommandContext> {
        [Command("read")]
        public async Task readImage() {
            var attachments = Context.Message.Attachments.GetEnumerator();
            if(attachments.MoveNext()) {
                string url = attachments.Current.Url;
                ImageReader reader = new ImageReader();
                await reader.ReadFromUrl(url);
                while(!reader.Ready) {
                    await Task.Delay(1);
                }
            }
        }

        [Command("read")]
        public async Task readImageUrl([Remainder]string url) {
            ImageReader reader = new ImageReader();
            await reader.ReadFromUrl(url);
        }
    }
}
