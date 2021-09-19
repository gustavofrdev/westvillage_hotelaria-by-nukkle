using System;
using System.Collections.Generic;
using CitizenFX.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using x_hotel_server.net.Framework;
using cfx = CitizenFX.Core.Native.API;
using Framework = x_hotel_server.net.Framework.FrameworkUtils;
using static x_hotel_server.net.ConfigManager.ConfigManager;

namespace x_hotel_server.net
{
    public class Main : BaseScript
    {
        private static readonly string Prefix = cfx.GetCurrentResourceName() + ":";

        public Main()
        {
            Initialize();
            EventHandlers[Prefix + "alugarHotel"] += new Action<string, int, string, string>(Check);
        }

        private void Check(string roomName, int source, string roomGroup, string hotelaria)
        {
            var framework = new FrameworkUtils();
            var userId = framework.GetUserId(source);
            // retrieve MYSQL //

            var sql = Exports["ghmattimysql"];
            Debug.WriteLine("Estou filtrando: " + hotelaria + " meu ID da framework é " + userId);
            sql.execute("SELECT quartosData FROM x_hotel WHERE hotel = ? ", new object[] {hotelaria},
                new Action<dynamic>(result =>
                {
                    if (result.Count >= 1)
                    {
                        var c = result[0];
                        if (c.quartosData != "[]")
                        {
                            var jsonObj = JsonConvert.DeserializeObject(c.quartosData);
                            //Debug.WriteLine(jsonObj[roomName].ToString());
                            if (jsonObj[roomName] != null)
                            {
                                Debug.WriteLine(jsonObj[roomName].ToString());
                                TriggerClientEvent("xFramework:Notify", "negado", " - Este quarto já está alugado.");
                            }
                            else
                            {
                                TriggerClientEvent("xFramework:Notify", "negado",
                                    " - Você alugou este quarto!\n O aluguel durará 7 dias, portanto, não deixe suas itens no baú após " +
                                    DateTime.Now.AddDays(7).ToString("dd/MM/yyyy HH:mm"));
                                var timeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                                var cd = new Dictionary<string, dynamic>
                                {
                                    ["Possuido"] = true,
                                    ["TempoAlugou"] = timeStamp,
                                    ["Em"] = hotelaria,
                                    ["PropiedadeTemporaria"] = userId
                                };
                                var b = JToken.FromObject(cd);
                                jsonObj[roomName] = b;
                                string ssys2 = JsonConvert.SerializeObject(jsonObj).ToString();
                                sql.execute("UPDATE x_hotel SET quartosData = ? WHERE hotel = ? ",
                                    new object[] {ssys2, hotelaria});
                                FrameworkUtils.SetUserGroup(userId, roomGroup, false);
                            }
                        }
                        else
                        {
                            TriggerClientEvent("xFramework:Notify", "negado",
                                " - Você alugou este quarto!\n O aluguel durará 7 dias, portanto, não deixe suas itens no baú após " +
                                DateTime.Now.AddDays(7).ToString("dd/MM/yyyy HH:mm"));
                            var timeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                            var b = new Dictionary<string, Dictionary<string, dynamic>>();
                            var cd = new Dictionary<string, dynamic>
                            {
                                ["Possuido"] = true,
                                ["TempoAlugou"] = timeStamp,
                                ["Em"] = hotelaria,
                                ["PropiedadeTemporaria"] = userId
                            };
                            b[roomName] = cd;
                            var jsonObj = JsonConvert.SerializeObject(b);
                            sql.execute("UPDATE x_hotel SET quartosData = ? WHERE hotel = ? ",
                                new object[] {jsonObj, hotelaria});
                            FrameworkUtils.SetUserGroup(userId, roomGroup, false);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Não temos dados desta hotelária no banco de dados...");
                    }
                }));
        }
    }
}