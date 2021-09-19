using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using x_hotel.net.ConfigManager;
using x_hotel.net.HotelFunctions;
using x_hotel.net.Misc;
using static x_hotel.net.Misc.Misc;
using static CitizenFX.Core.Native.API;
using Cfx = CitizenFX.Core.Native.API;

// ReSharper disable UnusedMember.Local
#pragma warning disable 1998


namespace x_hotel.net
{
    public class Main : BaseScript
    {
        private static JObject _hotelConfig;

        [Tick]
        private async Task OnStart()
        {
            await Delay(0);
            if (ConfigLoader.RecivedCfg)
            {
                Wait(5000);
                _hotelConfig = JsonUtils.ConvertStringToJObject(ConfigLoader.Cfg("Hotels", "*"));
                Tick -= OnStart;
                await Delay(0);
            }
        }

        private static void OnNui(string nuiAction)
        {
            SendNuiMessage(JsonConvert.SerializeObject(new
            {
                action = nuiAction
            }));
        }

        private static string OnReplace(string defaultm, string factor, string sub)
        {
            if (defaultm.Contains(factor)) defaultm = defaultm.Replace(factor, sub);
            return defaultm;
        }


        public static JObject GetConfigInstance()
        {
            return ConfigLoader.RecivedCfg ? _hotelConfig : new JObject();
        }

        [Tick]
        private static async Task OnShop()
        {
            var time = 1500; // otimization 
            var typeFinal = "Delay";
            if (ConfigLoader.RecivedCfg)
                for (var i = 0; i < _hotelConfig.Count; i++)
                    foreach (var i2 in _hotelConfig.Values())
                    {
                        //Debug.WriteLine(i2[i].ToString());
                        var x = float.Parse(i2[i]["ShopCds"][0].ToString());
                        var y = float.Parse(i2[i]["ShopCds"][1].ToString());
                        var z = float.Parse(i2[i]["ShopCds"][2].ToString()) - 1;
                        var playerCoords = GetEntityCoords(PlayerPedId(), false, false);
                        var msg = i2[i]["ShopMessage"].ToString();
                        msg = OnReplace(msg, "%loja%", i2[i]["Name"].ToString());
                        if (GetDistanceBetweenCoords(playerCoords.X, playerCoords.Y, playerCoords.Z, x, y, z,
                            true) < 5.0f)
                        {
                            if (IsControlJustPressed(0, 0x27D1C284))
                                Gui.SetupMenuForThisLoc(i2[i]["Name"].ToString(), i2[i]);
                            time = 0;
                            typeFinal = "!";
                            await DrawText3d(x, y, z, msg);
                        }

                        //Debug.WriteLine("HH: " + _hotelConfig.Count.ToString());
                        foreach (var b in i2[i]["Rooms"])
                            for (var bb = 0; bb < i2[i]["Rooms"][0].Count(); bb++)
                            {
                                var bedSleepingPos = new Vector3(
                                    float.Parse(b[bb.ToString()][0]["BedSleepingPos"][0].ToString()),
                                    float.Parse(b[bb.ToString()][0]["BedSleepingPos"][1].ToString()),
                                    float.Parse(b[bb.ToString()][0]["BedSleepingPos"][2].ToString()));
                                var bedSleepingHeading =
                                    float.Parse(b[bb.ToString()][0]["BedSleepingPos"][0].ToString());
                                var bedInteractPos = new Vector3(
                                    float.Parse(b[bb.ToString()][0]["BedInteract"][0].ToString()),
                                    float.Parse(b[bb.ToString()][0]["BedInteract"][1].ToString()),
                                    float.Parse(b[bb.ToString()][0]["BedInteract"][2].ToString()));
                                var plyCoords = GetEntityCoords(PlayerPedId(), false, false);
                                var dist = GetDistanceBetweenCoords(plyCoords.X, plyCoords.Y, plyCoords.Z,
                                    bedInteractPos.X, bedInteractPos.Y, bedInteractPos.Z, true);
                                if (!(dist < 2.0f)) continue;
                                var maisProx = bb;
                                Debug.WriteLine(maisProx.ToString());
                                bedInteractPos = new Vector3(
                                    float.Parse(b[maisProx.ToString()][0]["BedInteract"][0].ToString()),
                                    float.Parse(b[maisProx.ToString()][0]["BedInteract"][1].ToString()),
                                    float.Parse(b[maisProx.ToString()][0]["BedInteract"][2].ToString()));
                                time = 0;
                                typeFinal = "!";
                                await DrawText3d(bedInteractPos.X, bedInteractPos.Y, bedInteractPos.Z - 1.0f,
                                    "Pressione [R] para deitar\n[Backspace] - Levantar");
                                if (IsControlJustPressed(0, 0x27D1C284))
                                    Deitar.OnDeitar(bedSleepingPos.X, bedSleepingPos.Y, bedInteractPos.Z,
                                        bedSleepingHeading);
                            }
                    }

            if (typeFinal == "Delay")
                await Delay(time);
            else
                await Task.FromResult(0);
        }
    }
}