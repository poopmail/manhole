using Newtonsoft.Json;

namespace PoopmailGui.Api.Model
{
     public class MailboxResponse
        {
            [JsonProperty("data")]
            public Mailbox[] Data { get; set; }
    
            [JsonProperty("meta")]
            public Meta Meta { get; set; }
        }
    
        public class Mailbox
        {
            [JsonProperty("address")]
            public string Address { get; set; }
    
            [JsonProperty("account")]
            public string Account { get; set; }
    
            [JsonProperty("created")]
            public long Created { get; set; }
        }
    
        public partial class Meta
        {
            [JsonProperty("total")]
            public long Total { get; set; }
    
            [JsonProperty("displayed")]
            public long Displayed { get; set; }
        }
}