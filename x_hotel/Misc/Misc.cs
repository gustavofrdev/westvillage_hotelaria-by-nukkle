using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;

namespace x_hotel.net.Misc
{
    public class Misc : BaseScript
    {
        public static async Task DrawText3d(float x, float y, float z, string text)
        {
            float screenX = 0;
            float screenY = 0;
            var onScreen = GetScreenCoordFromWorldCoord(x, y, z + 1, ref screenX, ref screenY);
            var str = Function.Call<long>(Hash._CREATE_VAR_STRING, 10, "LITERAL_STRING", text);
            if (!onScreen) return;
            SetTextScale((float) 0.35, (float) 0.35);
            SetTextFontForCurrentCommand(1);
            SetTextColor(255, 255, 255, 215);
            SetTextCentre(true);
            SetTextDropshadow(1, 0, 0, 0, 255);
            DisplayText(str, screenX, screenY);
        }

        public static void NewCommand(Action<int, List<object>, string> a, string cmdName)
        {
            RegisterCommand(cmdName, new Action<int, List<object>, string>(a), false);
        }
    }
}