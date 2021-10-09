﻿using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Models.EventMicroservice
{
    public class Event
    {
        [JsonIgnore]
        [JsonProperty("objectId")]
        public ObjectId Id { get; set; }


        [JsonProperty("code")]
        public string Code { get; set; }


        [JsonProperty("name")]
        public string Name { get; set; }


        [JsonProperty("dateTimeOfEvent")]
        public DateTime DateTimeOfEvent { get; set; }


        [JsonProperty("interestIds")]
        public IList<int> InterestIds { get; set; }

        [JsonIgnore]
        [JsonProperty("video")]
        public byte[] Video { get; set; }
    }
}
