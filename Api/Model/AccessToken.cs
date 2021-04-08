using Newtonsoft.Json;

namespace PoopmailGui.Api.Model
{
    public class AccessToken
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }

        [JsonProperty("expires")]
        public long Expires { get; set; }
    }
}