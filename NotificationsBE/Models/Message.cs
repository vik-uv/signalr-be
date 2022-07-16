using Newtonsoft.Json;
using System;
namespace NotificationsBE.Models
{
    public class Message
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("datetime")]
        public DateTime? DateTime { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
