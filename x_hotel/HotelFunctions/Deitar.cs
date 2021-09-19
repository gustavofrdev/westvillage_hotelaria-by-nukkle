using CitizenFX.Core;
using cfx = CitizenFX.Core.Native.API;

#pragma warning disable 1998

namespace x_hotel.net.HotelFunctions
{
    public class Deitar : BaseScript
    {
        public static void OnDeitar(float x, float y, float z, float heading)
        {
            var deitarPos = new Vector3(x, y, z);
            const string deitarCommand = "deitar3";
            cfx.SetEntityCoords(cfx.PlayerPedId(), deitarPos.X, deitarPos.Y, deitarPos.Z, true, true, true, false);
            cfx.SetEntityHeading(cfx.PlayerPedId(), heading);
            cfx.ExecuteCommand(deitarCommand);
        }
    }
}