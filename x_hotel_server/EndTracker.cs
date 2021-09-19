using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Debug = CitizenFX.Core.Debug;

#pragma warning disable 1998

namespace x_hotel_server.net
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public class EndTracker : BaseScript
    {
        private Dictionary<int, Dictionary<string, long>> _dados = new Dictionary<int, Dictionary<string, long>>();

        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        [Tick]
        private async Task Tracker()
        {
            var timeStampToday = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var sql = Exports["ghmattimysql"];
            sql.execute("SELECT quartosData FROM x_hotel ", new[] {""}, new Action<dynamic>(result =>
            {
                if (result.Count < 1) return;
                for (var mi = 0; mi < result.Count; mi++)
                {
                    var c = result[mi].quartosData;
                    if (c.ToString() == "[]") continue;
                    JObject jsonObj = JsonConvert.DeserializeObject(c);
                    string final;

                    IList<string> keys = jsonObj.Properties().Select(p => p.Name).ToList();
                    for (var ssy = 0; ssy < keys.Count(); ssy++)
                        try
                        {
                            var timeStampTo = (long) jsonObj[keys[ssy]]["TempoAlugou"];
                            var a = UnixTimeStampToDateTime(timeStampTo);
                            var b = UnixTimeStampToDateTime(timeStampToday);
                            var nomeHotelaria = (string) jsonObj[keys[ssy]]["Em"];
                            var tempoNoQuarto = Convert.ToInt32((a - b).TotalDays) * -1;
                            if (tempoNoQuarto < 0) continue;
                            jsonObj.Remove(keys[ssy]);
                            final = jsonObj.ToString();
                            if (jsonObj.ToString() == "{}") final = "[]";
                            sql.execute("UPDATE x_hotel SET quartosData = ? WHERE hotel = ? ",
                                new object[] {final, nomeHotelaria});
                        }
                        catch (Exception e)
                        {
                            var st = new StackTrace(e, true);
                            var frame = st.GetFrame(0);
                            var line = frame.GetFileLineNumber();
                            Debug.WriteLine("tipo invalido 0 + " + e.Message + " linha do erro: " + line);
                        }
                }
            }));
            await Delay(10000);
        }
    }
}