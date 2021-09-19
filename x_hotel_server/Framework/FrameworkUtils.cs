using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json.Linq;

namespace x_hotel_server.net.Framework
{
    public class FrameworkUtils : BaseScript

    {
        private const string EventPass = "R%xunmAy^8^H8#edUG";

        public static void SetUserGroup(int userId, string group, bool log)
        {
            TriggerEvent("SetUserGroup-Cs", userId, group, log, EventPass);
        }

        public static void RemoveUserGroup(int userId, string group, bool log)
        {
            TriggerEvent("RemoveUserGroup-Cs", userId, group, log, EventPass);
        }

        public bool CheckItem(int userId, string itemName, int quantidade)
        {
            return Exports["_xFramework"].TryGetInventoryItem(userId, itemName, quantidade);
        }

        private async Task<List<string>> GetUserGroups(int userId)
        {
            var playerGroups = new List<string>();
            List<dynamic> result = await Exports["ghmattimysql"]
                .executeSync("SELECT grupos FROM xframework_personagens WHERE id = ?", new object[] {userId});
            string gps = result[0].grupos;
            var objects = JObject.Parse(gps);
            if (objects.Count < 0) return playerGroups;
            foreach (var o in objects) playerGroups.Add(o.Key);
            return playerGroups;
        }

        public async Task<bool> HasPermission(int id, string perm, dynamic groupsCfg)
        {
            var hasPermission = false;
            var userGroups = await GetUserGroups(id);
            JObject groupsObjects = JObject.Parse(groupsCfg);
            foreach (var n in userGroups)
            {
                if (groupsObjects["Grupos"][n] == null) return false;
                foreach (var g in groupsObjects["Grupos"][n]["permissoes"])
                    if (g.ToString() == perm)
                        hasPermission = true;
            }

            return hasPermission;
        }

        public int GetUserId(int src)
        {
            return Exports["_xFramework"].GetUserId(src);
        }

        public int GetUserSource(int id)
        {
            return Exports["_xFramework"].GetUserSource(id);
        }
    }
}