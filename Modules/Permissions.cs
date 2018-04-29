using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GarlicBot.Modules
{
    public class PermissionsManager
    {

        static PermissionsManager() {
            if (!Directory.Exists(_folder)) {
                Directory.CreateDirectory(_folder);
            }
            if(!File.Exists($"{_folder}/{_permsFile}")) {
                Initialize();
            }
            LoadPerms();
            LoadGroups();
        }

        public static async Task<bool> GetPerm(ulong id, Permissions perm) {
            await Task.Delay(1); // TODO: Find a much better way to get rid of warning

            // first check groups to see if the user is in any groups
            // the user can be in multiple groups. if any of the groups they
            // are in has the given permission, they have that permission, even
            // if another group they are in does NOT have that permission
            foreach(var group in _groups) {
                if(group.Value.CheckUser(id)) {
                    if(group.Value.GetPerm(perm)) {
                        return true;
                    }
                }
            }

            // if we got this far, the user is either not in any groups
            // or is not in a group with the given permission. we can still check
            // their invdividual permissions, and if they don't have any, we revert
            // to the last case scenario of the default perms
            UserPerms perms;
            if (_perms.ContainsKey(id)) {
                perms = _perms[id];
            }
            else {
                perms = _perms[0];
            }
            if (perms == null)
                return false;
            switch (perm) {
                case Permissions.Shutdown:
                    return perms.shutdown;
                case Permissions.Restart:
                    return perms.restart;
                case Permissions.MakeQuote:
                    return perms.makeQuote;
                case Permissions.GetQuote:
                    return perms.getQuote;
                case Permissions.ProcessImage:
                    return perms.processImage;
                case Permissions.PermissionManagement:
                    return perms.permissionManagement;
                default:
                    return false;
            }
        }

        public static async Task<bool> AddPerms(ulong id, Permissions perm) {
            UserPerms perms;

            if (_perms.ContainsKey(id)) {
                perms = _perms[id];
            }
            else {
                perms = new UserPerms();
                perms.userID = id;
                _perms.Add(id, perms);
            }

            switch (perm) {
                case Permissions.Shutdown:
                    perms.shutdown = true;
                    break;
                case Permissions.Restart:
                    perms.restart = true;
                    break;
                case Permissions.MakeQuote:
                    perms.makeQuote = true;
                    break;
                case Permissions.GetQuote:
                    perms.getQuote = true;
                    break;
                case Permissions.ProcessImage:
                    perms.processImage = true;
                    break;
                case Permissions.PermissionManagement:
                    perms.permissionManagement = false;
                    break;
                default:
                    return false;
            }
            await SavePerms();

            return true;
        }

        public static async Task<bool> RemovePerms(ulong id, Permissions perm) {
            UserPerms perms;

            if (_perms.ContainsKey(id)) {
                perms = _perms[id];
            }
            else {
                return true;
            }

            switch (perm) {
                case Permissions.Shutdown:
                    perms.shutdown = false;
                    break;
                case Permissions.Restart:
                    perms.restart = false;
                    break;
                case Permissions.MakeQuote:
                    perms.makeQuote = false;
                    break;
                case Permissions.GetQuote:
                    perms.getQuote = false;
                    break;
                case Permissions.ProcessImage:
                    perms.processImage = false;
                    break;
                case Permissions.PermissionManagement:
                    perms.permissionManagement = false;
                    break;
                default:
                    return false;
            }
            await SavePerms();

            return true;
        }

        public static bool CreateGroup(string name, string error) {
            if(_groups.ContainsKey(name.ToLower())) {
                error = "Group already exists!";
                return false;
            }
            PermissionGroup newGroup = new PermissionGroup();
            _groups.Add(name.ToLower(), newGroup);
            SaveGroups();
            return true;
        }

        public static bool EditGroup(string group, Permissions perm, bool status, string error) {
            if(!_groups.ContainsKey(group.ToLower())) {
                error = "Group does not exist!";
                return false;
            }
            PermissionGroup groupToEdit = _groups[group.ToLower()];
            if(!groupToEdit.SetPerm(status, perm)) {
                error = "Invalid permission!";
                return false;
            }
            SaveGroups();
            return true;
        }

        public static bool AddUserToGroup(ulong id, string group, string error) {
            if (!_groups.ContainsKey(group.ToLower())) {
                error = "Group does not exist!";
                return false;
            }
            PermissionGroup groupToEdit = _groups[group.ToLower()];
            if(!groupToEdit.AddUser(id)) {
                error = "User already in group!";
                return false;
            }
            SaveGroups();
            return true;
        }

        public static bool RemoveUserFromGroup(ulong id, string group, string error) {
            if (!_groups.ContainsKey(group.ToLower())) {
                error = "Group does not exist!";
                return false;
            }
            PermissionGroup groupToEdit = _groups[group.ToLower()];
            if (!groupToEdit.RemoveUser(id)) {
                error = "User is not in that group!";
                return false;
            }
            SaveGroups();
            return true;
        }

        public static bool RemoveGroup(string name, string error) {
            if(_groups.ContainsKey(name.ToLower())) {
                _groups.Remove(name.ToLower());
                SaveGroups();
                return true;
            }
            else {
                error = "Group does not exist!";
                return false;
            }
        }

        public static void SaveGroups() {
            string json = JsonConvert.SerializeObject(_groups, Formatting.Indented);
            File.WriteAllText($"{_folder}/{_groupsFile}", json);
        }

        public static void LoadGroups() {
            if(!File.Exists($"{_folder}/{_groupsFile}")) {
                _groups = new Dictionary<string, PermissionGroup>();
            }
            else {
                string json = File.ReadAllText($"{_folder}/{_groupsFile}");
                _groups = JsonConvert.DeserializeObject<Dictionary<string, PermissionGroup>>(json);
            }
        }

        private static async Task SavePerms() {
            string json = JsonConvert.SerializeObject(_perms, Formatting.Indented);
            await File.WriteAllTextAsync($"{_folder}/{_permsFile}", json);
        }

        private static void LoadPerms() {
            string json = File.ReadAllText($"{_folder}/{_permsFile}");
            _perms = JsonConvert.DeserializeObject<Dictionary<ulong, UserPerms>>(json);
        }

        private static void Initialize() {
            // this will always create a set of permissions with userid = 0
            // the set of permissions with userid = 0 will act as the default permissions
            // for users who are not in a permission group or do not have their own permission profile
            Dictionary<ulong, UserPerms> temp = new Dictionary<ulong, UserPerms>();
            UserPerms basePerms = new UserPerms();
            temp.Add(0, basePerms);
            foreach (ulong i in Config.bot.adminUsers) {
                UserPerms perms = new UserPerms();
                perms.userID = i;
                perms.getQuote = true;
                perms.shutdown = true;
                perms.processImage = true;
                perms.restart = true;
                perms.makeQuote = true;
                perms.permissionManagement = true;
                temp.Add(i, perms);
            }
            string json = JsonConvert.SerializeObject(temp, Formatting.Indented);
            File.WriteAllText($"{_folder}/{_permsFile}", json);
        }

        private static Dictionary<ulong, UserPerms> _perms;
        private static Dictionary<string, PermissionGroup> _groups;

        public static Dictionary<string, PermissionGroup> Groups {
            get {
                return _groups;
            }
        }

        private static string _folder = "Resources";
        private static string _permsFile = "permissions.json";
        private static string _groupsFile = "permissiongroups.json";
    }

    public enum Permissions { Shutdown, Restart, MakeQuote, GetQuote, ProcessImage, PermissionManagement }

    public class UserPerms {
        public ulong userID = 0;
        public bool shutdown = false;
        public bool restart = false;
        public bool makeQuote = false;
        public bool getQuote = false;
        public bool processImage = false;
        public bool permissionManagement = false;
    }

    public class PermissionGroup {
        public PermissionGroup() {
            members = new List<ulong>();
            perms = new UserPerms();
        }

        public bool SetPerm(bool status, Permissions perm) {
            switch (perm) {
                case Permissions.Shutdown:
                    perms.shutdown = status;
                    return true;
                case Permissions.Restart:
                    perms.restart = status;
                    return true;
                case Permissions.MakeQuote:
                    perms.makeQuote = status;
                    return true;
                case Permissions.GetQuote:
                    perms.getQuote = status;
                    return true;
                case Permissions.ProcessImage:
                    perms.processImage = status;
                    return true;
                case Permissions.PermissionManagement:
                    perms.permissionManagement = status;
                    return true;
                default:
                    return false;
            }
        }

        public bool GetPerm(Permissions perm) {
            switch (perm) {
                case Permissions.Shutdown:
                    return perms.shutdown;
                case Permissions.Restart:
                    return perms.restart;
                case Permissions.MakeQuote:
                    return perms.makeQuote;
                case Permissions.GetQuote:
                    return perms.getQuote;
                case Permissions.ProcessImage:
                    return perms.processImage;
                case Permissions.PermissionManagement:
                    return perms.permissionManagement;
                default:
                    return false;
            }
        }

        public bool AddUser(ulong id) {
            if (members.Contains(id))
                return false;
            members.Add(id);
            return true;
        }

        public bool RemoveUser(ulong id) {
            if (!members.Contains(id))
                return false;
            members.Remove(id);
            return true;
        }

        public bool CheckUser(ulong id) {
            return members.Contains(id);
        }

        public UserPerms perms;
        public List<ulong> members;
    }
}
