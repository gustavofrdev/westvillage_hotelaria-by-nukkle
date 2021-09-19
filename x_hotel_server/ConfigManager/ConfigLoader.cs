using CitizenFX.Core;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedType.Global

namespace x_hotel_server.net.ConfigManager
{
    public class ConfigLoader : BaseScript
    {
        public static string GetConfig(JObject configObject, int leng, string configBase, string configSel)
        {
            return configObject[configBase][leng][configSel].ToString();
        }
    }
}