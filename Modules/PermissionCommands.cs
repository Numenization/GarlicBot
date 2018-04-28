using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace GarlicBot.Modules
{
    public class PermissionCommands : ModuleBase<SocketCommandContext> {
        [Command("setperm")]
        public async Task SetPerm(params string[] args) {
            SocketUser commandUser = Context.User;
            ulong commandId = commandUser.Id;
            if (!await PermissionsManager.GetPerm(commandId, Permissions.PermissionManagement)) {
                await Utilities.SendMessage(
                    await Utilities.GetAlert("invalidPerms"), // message body
                    await Utilities.GetAlert("commandErrorTitle"), // message title
                    Context); // command context
                return;
            }

            if (args.Length >= 2) {
                string[] userparams = args[0].Split('#');
                if (userparams.Length != 2)
                    return; // invalid username0
                string username = userparams[0];
                string discriminator = userparams[1];
                Permissions perm;

                // get the user's id
                SocketUser user = Context.Client.GetUser(username, discriminator);
                ulong id = user.Id;

                // get the correct permission
                switch (args[1].ToLower()) {
                    case "shutdown":
                        perm = Permissions.Shutdown;
                        break;
                    case "restart":
                        perm = Permissions.Restart;
                        break;
                    case "makequote":
                        perm = Permissions.MakeQuote;
                        break;
                    case "getquote":
                        perm = Permissions.GetQuote;
                        break;
                    case "processimage":
                        perm = Permissions.ProcessImage;
                        break;
                    case "permissions":
                        perm = Permissions.PermissionManagement;
                        break;
                    default:
                        // invalid permission
                        return;
                }

                // give the user the permission
                await PermissionsManager.AddPerms(id, perm);
            }
            else {
                // invalid arguments
            }
        }
    }
}
