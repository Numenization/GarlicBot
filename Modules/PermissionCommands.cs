using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace GarlicBot.Modules
{
    public class PermissionCommands : ModuleBase<SocketCommandContext> {
        [Command("perms")]
        public async Task Perms(params string[] args) {
            SocketUser commandUser = Context.User;
            ulong commandId = commandUser.Id;
            if (!await PermissionsManager.GetPerm(commandId, Permissions.PermissionManagement)) {
                await Utilities.SendMessage(
                    await Utilities.GetAlert("invalidPerms"), // message body
                    await Utilities.GetAlert("commandErrorTitle"), // message title
                    Context); // command context
                return;
            }

            try {
                if(args[0] == "set") {
                    // perms set username perm status
                    string userMention = args[1];
                    if (!(userMention.Substring(0, 2).Equals("<@") && userMention[userMention.Length - 1].Equals('>'))) {
                        await Utilities.SendMessage(
                            $"Couldn't get user",
                            "Permissions:",
                            Context);
                        return;
                    }

                    ulong id = ulong.Parse(userMention.Substring(2, userMention.Length - 3));
                    SocketUser user = Context.Client.GetUser(id);
                    Permissions perm;
                    bool status = bool.Parse(args[3]);

                    switch (args[2].ToLower()) {
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
                        case "permissionmanagement":
                            perm = Permissions.PermissionManagement;
                            break;
                        default:
                            await Utilities.SendMessage(
                                $"Invalid permission",
                                "Permissions:",
                                Context);
                            return;
                    }

                    if (status) {
                        await PermissionsManager.AddPerms(id, perm);
                    }
                    else {
                        await PermissionsManager.RemovePerms(id, perm);
                    }
                }
                else if(args[0] == "get") {
                    // perms get username perm
                    string userMention = args[1];
                    if (!(userMention.Substring(0, 2).Equals("<@") && userMention[userMention.Length - 1].Equals('>'))) {
                        await Utilities.SendMessage(
                            $"Couldn't get user",
                            "Permissions:",
                            Context);
                        return;
                    }

                    ulong id = ulong.Parse(userMention.Substring(2, userMention.Length - 3));
                    SocketUser user = Context.Client.GetUser(id);
                    Permissions perm;

                    switch (args[2].ToLower()) {
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
                        case "permissionmanagement":
                            perm = Permissions.PermissionManagement;
                            break;
                        default:
                            await Utilities.SendMessage(
                                $"Invalid permission",
                                "Permissions:",
                                Context);
                            return;
                    }
                    bool status = await PermissionsManager.GetPerm(id, perm);

                    await Utilities.SendMessage(
                        $"Permission {perm} for {user.Username}#{user.Discriminator} = {status}",
                        "Permissions:",
                        Context);
                }
                else if(args[0] == "list") {
                    string permissions = "";
                    foreach (Permissions perm in Enum.GetValues(typeof(Permissions))) {
                        permissions += $"{perm} ";
                    }

                    await Utilities.SendMessage(
                        $"`{permissions}`",
                        "Permission List:",
                        Context);
                }
            }
            catch {
                await Utilities.SendMessage(
                    String.Format(await Utilities.GetAlert("commandInvalidArgs"), "setperm"), // message body
                    await Utilities.GetAlert("commandErrorTitle"), // message title
                    Context); // command context
            }
        }

        [Command("group")]
        public async Task Group(params string[] args) {
            // usage: group action arg1 arg2
            SocketUser commandUser = Context.User;
            ulong commandId = commandUser.Id;
            if (!await PermissionsManager.GetPerm(commandId, Permissions.PermissionManagement)) {
                await Utilities.SendMessage(
                    await Utilities.GetAlert("invalidPerms"), // message body
                    await Utilities.GetAlert("commandErrorTitle"), // message title
                    Context); // command context
                return;
            }

            // get action
            try {
                if(args[0] == "create") {
                    // create a group
                    // group create groupname
                    string groupName = args[1];
                    string error = "";
                    if (!PermissionsManager.CreateGroup(groupName, error)) {
                        await Utilities.SendMessage(
                            error, // message body
                            await Utilities.GetAlert("commandErrorTitle"), // message title
                            Context); // command context
                        return;
                    }

                    await Utilities.SendMessage(
                        $"Group \"{groupName}\" added",
                        "Permissions:",
                        Context);
                }
                else if(args[0] == "delete") {
                    // delete a group
                    // group delete groupname
                    string groupName = args[1];
                    string error = "";
                    if (!PermissionsManager.RemoveGroup(groupName, error)) {
                        await Utilities.SendMessage(
                            error, // message body
                            await Utilities.GetAlert("commandErrorTitle"), // message title
                            Context); // command context
                        return;
                    }
                    await Utilities.SendMessage(
                        $"Group \"{groupName}\" removed",
                        "Permissions:",
                        Context);
                }
                else if(args[0] == "add") {
                    // add a user to a group
                    // group add groupname @username#discriminator
                    string groupName = args[1];
                    string userMention = args[2];
                    if (!(userMention.Substring(0, 2).Equals("<@") && userMention[userMention.Length - 1].Equals('>'))) {
                        await Utilities.SendMessage(
                            $"Couldn't get user",
                            "Permissions:",
                            Context);
                        return;
                    }

                    ulong id = ulong.Parse(userMention.Substring(2, userMention.Length - 3));
                    SocketUser user = Context.Client.GetUser(id);
                    string error = "";
                    if(!PermissionsManager.AddUserToGroup(id, groupName, error)) {
                        await Utilities.SendMessage(
                            error, // message body
                            await Utilities.GetAlert("commandErrorTitle"), // message title
                            Context); // command context
                        return;
                    }

                    await Utilities.SendMessage(
                        $"{user.Username}#{user.Discriminator} added to {groupName}",
                        "Permissions:",
                        Context);
                }
                else if(args[0] == "remove") {
                    // remove a user from a group
                    // group remove groupname @username#discriminator
                    string groupName = args[1];
                    string userMention = args[2];
                    if (!(userMention.Substring(0, 2).Equals("<@") && userMention[userMention.Length - 1].Equals('>'))) {
                        await Utilities.SendMessage(
                            $"Couldn't get user",
                            "Permissions:",
                            Context);
                        return;
                    }

                    ulong id = ulong.Parse(userMention.Substring(2, userMention.Length - 3));
                    SocketUser user = Context.Client.GetUser(id);
                    string error = "";
                    if (!PermissionsManager.RemoveUserFromGroup(id, groupName, error)) {
                        await Utilities.SendMessage(
                            error, // message body
                            await Utilities.GetAlert("commandErrorTitle"), // message title
                            Context); // command context
                        return;
                    }

                    await Utilities.SendMessage(
                        $"{user.Username}#{user.Discriminator} removed from {groupName}",
                        "Permissions:",
                        Context);
                }
                else if(args[0] == "set") {
                    // group set groupname permission status
                    string groupName = args[1];
                    bool status = bool.Parse(args[3]);
                    string error = "";
                    Permissions perm;

                    // get the correct permission
                    switch (args[2].ToLower()) {
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
                        case "permissionmanagement":
                            perm = Permissions.PermissionManagement;
                            break;
                        default:
                            await Utilities.SendMessage(
                                $"Invalid permission",
                                "Permissions:",
                                Context);
                            return;
                    }

                    if (!PermissionsManager.EditGroup(groupName, perm, status, error)) {
                        await Utilities.SendMessage(
                            error, // message body
                            await Utilities.GetAlert("commandErrorTitle"), // message title
                            Context); // command context
                        return;
                    }

                    await Utilities.SendMessage(
                        $"Permission {perm} set to {status} for {groupName}",
                        "Permissions:",
                        Context);
                }
                else if(args[0] == "get") {
                    // group get groupname perm
                    string groupName = args[1];
                    Permissions perm;

                    // get the correct permission
                    switch (args[2].ToLower()) {
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
                        case "permissionmanagement":
                            perm = Permissions.PermissionManagement;
                            break;
                        default:
                            await Utilities.SendMessage(
                                $"Invalid permission",
                                "Permissions:",
                                Context);
                            return;
                    }

                    if (!PermissionsManager.Groups.ContainsKey(groupName)) {
                        await Utilities.SendMessage(
                            "Group does not exist!", // message body
                            await Utilities.GetAlert("commandErrorTitle"), // message title
                            Context); // command context
                        return;
                    }

                    bool status = PermissionsManager.Groups[groupName].GetPerm(perm);

                    await Utilities.SendMessage(
                        $"Permission {perm} for group {groupName} = {status}",
                        "Permissions:",
                        Context);
                }
                else if(args[0] == "members") {
                    // gets members in a group
                    // group members groupname
                    string groupName = args[1];
                    if (!PermissionsManager.Groups.ContainsKey(groupName)) {
                        await Utilities.SendMessage(
                            "Group does not exist!", // message body
                            await Utilities.GetAlert("commandErrorTitle"), // message title
                            Context); // command context
                        return;
                    }

                    PermissionGroup group = PermissionsManager.Groups[groupName];
                    string users = "";
                    foreach(ulong id in group.members) {
                        users += $"{Context.Client.GetUser(id).Username}#{Context.Client.GetUser(id).Discriminator} ";
                    }

                    await Utilities.SendMessage(
                        $"{groupName}: `{users}`",
                        "Permissions:",
                        Context);
                }
                else if(args[0] == "getperms") {
                    // group getperms groupname
                    string groupName = args[1];
                    if (!PermissionsManager.Groups.ContainsKey(groupName)) {
                        await Utilities.SendMessage(
                            "Group does not exist!", // message body
                            await Utilities.GetAlert("commandErrorTitle"), // message title
                            Context); // command context
                        return;
                    }

                    PermissionGroup group = PermissionsManager.Groups[groupName];
                    string permissions = "";
                    foreach (Permissions perm in Enum.GetValues(typeof(Permissions))) {
                        if(group.GetPerm(perm)) {
                            permissions += $"{perm} ";
                        }
                    }

                    await Utilities.SendMessage(
                        $"`{permissions}`",
                        "Permission Groups:",
                        Context);
                }
                else if(args[0] == "list") {
                    string groups = String.Join(", ", PermissionsManager.Groups.Keys);

                    await Utilities.SendMessage(
                        $"`{groups}`",
                        "Permission Groups:",
                        Context);
                }
            }
            catch {
                await Utilities.SendMessage(
                    String.Format(await Utilities.GetAlert("commandInvalidArgs"), "setperm"), // message body
                    await Utilities.GetAlert("commandErrorTitle"), // message title
                    Context); // command context
            }
        }
    }
}
