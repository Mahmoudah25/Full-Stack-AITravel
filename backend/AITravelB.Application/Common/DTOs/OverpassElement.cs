using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AITravelB.Application.Common.DTOs
{
    public class OverpassElement
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("lat")]
        public double Lat { get; set; }
        [JsonPropertyName("lon")]
        public double Lon { get; set; }
        [JsonPropertyName("tags")]
        public Dictionary<string, string>? Tags { get; set; }
    }
}
