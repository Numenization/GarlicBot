using System;
using System.Collections.Generic;
using System.IO;
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
        }

        public static bool GetPerm(ulong id, Permissions perm) {
            UserPerms perms;
            if(_perms.ContainsKey(id)) {
                perms = _perms[id];
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
            else {
                return false;
            }
        }

        public static bool AddPerms(ulong id, Permissions perm) {
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
            SavePerms();

            return true;
        }

        public static bool RemovePerms(ulong id, Permissions perm) {
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

            return true;
        }

        private static void SavePerms() {
            string json = JsonConvert.SerializeObject(_perms);
            File.WriteAllText($"{_folder}/{_permsFile}", json);
        }

        private static void LoadPerms() {
            string json = File.ReadAllText($"{_folder}/{_permsFile}");
            _perms = JsonConvert.DeserializeObject<Dictionary<ulong, UserPerms>>(json);
        }

        private static void Initialize() {
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

        private static string _folder = "Resources";
        private static string _permsFile = "permissions.json";
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
}
