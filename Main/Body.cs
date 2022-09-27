using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NextBuses
{
    public class Body
    {
        [JsonPropertyName("requestedStops")]
        public List<RequestedStop> RequestedStops { get; set; }
    }
    public class RequestedStop
    {
        [JsonPropertyName("id")]
        public string StopID { get; set; }
        [JsonPropertyName("Lines")]
        public List<string> Lines { get; set; }
        [JsonPropertyName("Directions")]
        public List<string> Directions { get; set; }
    }
}

