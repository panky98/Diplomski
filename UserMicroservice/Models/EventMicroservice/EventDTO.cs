using System;
using System.Collections.Generic;
using System.Text;

namespace Models.EventMicroservice
{
    public class EventDTO
    {
        public string Name { get; set; }
        public DateTime DateTimeOfEvent { get; set; }
        public IList<int> InterestIds { get; set; }
        public string Base64 { get; set; }
    }
}
