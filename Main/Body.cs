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
        [JsonPropertyName("lines")]
        public List<string> Lines { get; set; }
        [JsonPropertyName("directions")]
        public List<string> Directions { get; set; }
        [JsonPropertyName("excludedTransportTypes")]
        public List<TransportType> ExcludedTransportTypes { get; set; }
    }
    public enum TransportType
    {
        Bus = 0,
        Train = 1
    }
}

