using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using MenuAPI;
using Newtonsoft.Json.Linq;
using cfx = CitizenFX.Core.Native.API;

namespace x_hotel.net.Misc
{
    public class Gui : BaseScript
    {
        private static readonly string ClientPrefix = cfx.GetCurrentResourceName() + ":";

        public static void SetupMenuForThisLoc(string location, JToken jsonObject)
        {
            var room = jsonObject["Rooms"][0];
            var menu = new Menu("Hotelária", location);
            MenuController.AddMenu(menu);
            MenuController.MenuToggleKey = 0;
            var roomsLength = room.Count();
            var roomNames = new List<string>();
            for (var i = 0; i < roomsLength; i++) roomNames.Add(room[i.ToString()][0]["DisplayName"].ToString());
            var display = new MenuListItem("Quartos", roomNames, 0, "Todos os quartos de " + location);
            menu.AddMenuItem(display);
            menu.OpenMenu();
            menu.OnListItemSelect += (__, listItem, listIndex, realIndex) =>
            {
                var confirmator = new Menu("Comprar ", "");
                var sim = new MenuItem("Alugar " + roomNames[listIndex], "Sim");
                var nao = new MenuItem("Sair", "Não");
                confirmator.AddMenuItem(sim);
                confirmator.AddMenuItem(nao);
                MenuController.BindMenuItem(menu, confirmator, display);
                confirmator.OnItemSelect += (sender, item, _) =>
                {
                    switch (item.Text)
                    {
                        case "Sair":
                            confirmator.CloseMenu();
                            menu.OpenMenu();
                            break;
                        default:
                            if (item.Text.Contains("Alugar"))
                            {
                                //botão é seguro 
                                var group = room[listIndex.ToString()][0]["GroupSet"].ToString();
                                TriggerServerEvent(ClientPrefix + "alugarHotel", roomNames[listIndex],
                                    cfx.GetPlayerServerId(cfx.PlayerId()), group, location);
                            }

                            break;
                    }
                };
            };
        }
    }
}