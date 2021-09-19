using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json.Linq;

// ReSharper disable ClassNeverInstantiated.Global

namespace x_hotel.net.ConfigManager
{
    public class ConfigLoader : BaseScript
    {
        public static bool RecivedCfg;
        private static JObject _config;

        public ConfigLoader()
        {
            EventHandlers[$"{API.GetCurrentResourceName()}:Config"] += new Action<string>(ReciveConfig);
            TriggerServerEvent($"{API.GetCurrentResourceName()}:SendConfig");
        }

        private static void ReciveConfig(string config)
        {
            RecivedCfg = true;
            var cfg = JObject.Parse(config);
            _config = cfg;
        }

        public static string Cfg(string basecfg, string cfg)
        {
            return cfg == "*" ? _config[basecfg][0].ToString() : _config[basecfg][0][cfg].ToString();
        }
    }
}