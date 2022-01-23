using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.EventMicroservice
{
    public class EventDTO
    {
        [JsonProperty("creatorId")]
        public int CreatorId { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("dateTimeOfEvent")]

        public DateTime DateTimeOfEvent { get; set; }
        [JsonProperty("interestIds")]
        public IList<int> InterestIds { get; set; }
        [JsonProperty("base64")]
        public string Base64 { get; set; }

        public IList<int> UserIds { get; set; }
    }
}
