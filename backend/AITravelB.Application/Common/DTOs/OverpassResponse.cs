using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AITravelB.Application.Common.DTOs
{
    public class OverpassResponse
    {
        [JsonPropertyName("elements")]
        public List<OverpassElement> Elements { get; set; }
    }
}
