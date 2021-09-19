using System;
using System.IO;
using CitizenFX.Core;
using Newtonsoft.Json.Linq;
using static CitizenFX.Core.Native.API;
using static x_hotel_server.net.Misc.EasyUtils;

// ReSharper disable ClassNeverInstantiated.Global

namespace x_hotel_server.net.ConfigManager
{
    public class ConfigManager : BaseScript
    {
        private static JObject _configFile;

        public ConfigManager()
        {
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            EventHandlers[$"{GetCurrentResourceName()}:SendConfig"] += new Action<Player>(SendConfigFile);
        }

        public static void Initialize()
        {
            var path = $"{GetResourcePath(GetCurrentResourceName())}/config.json";
            if (File.Exists(path))
            {
                var readedFile = File.ReadAllText(path);
                _configFile = JObject.Parse(readedFile);
                ColorDebug(ConsoleColor.Green, $"{GetCurrentResourceName()}: Configurações[JSON] carregadas!");
            }
            else
            {
                ColorDebug(ConsoleColor.Red,
                    $"{GetCurrentResourceName()}: Não é possível carregar o arquivo JSON de configurações do sccript");
            }
        }

        public static JObject RetrieveConfigFile()
        {
            return _configFile;
        }

        public static JObject CfgObject()
        {
            var path = $"{GetResourcePath(GetCurrentResourceName())}/Config.json";
            if (!File.Exists(path)) return new JObject();
            var readedFile = File.ReadAllText(path);
            _configFile = JObject.Parse(readedFile);
            return _configFile;
        }


        private static void SendConfigFile([FromSource] Player player)
        {
            player.TriggerEvent($"{GetCurrentResourceName()}:Config", _configFile.ToString());
        }

        public static string Cfg(string basecfg, string cfg)
        {
            return cfg == "*" ? _configFile[basecfg][0].ToString() : _configFile[basecfg][0][cfg].ToString();
        }
    }
}