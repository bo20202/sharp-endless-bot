using Newtonsoft.Json;

namespace EndlessConfiguration.Models
{
    public class ServerInfo
    {
        [JsonIgnore]
        public Server Server { get; set; }

        [JsonProperty("players")]
        public int Players { get; set; }
        [JsonProperty("admins")]
        public int Admins { get; set; }
        [JsonProperty("isOnline")]
        public bool IsOnline { get; set; }

    }
}