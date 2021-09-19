using Newtonsoft.Json.Linq;

namespace x_hotel.net.ConfigManager
{
    public static class JsonUtils
    {
        public static JObject ConvertStringToJObject(string request)
        {
            return JObject.Parse(request);
        }
    }
}