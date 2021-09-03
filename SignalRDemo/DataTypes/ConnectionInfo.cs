using Newtonsoft.Json;

namespace SignalRDemo.DataTypes
{
    public class ConnectionInfo
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
    }
}