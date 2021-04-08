using Newtonsoft.Json;

namespace PoopmailGui.Api.Model
{
    public partial class MessageResponse
    {
        [JsonProperty("data")]
        public Message[] Data { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    
    public partial class Message
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("mailbox")]
        public string Mailbox { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("content")]
        public Content Content { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }
    }

    public partial class Content
    {
        [JsonProperty("plain")]
        public string Plain { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }
    }
}