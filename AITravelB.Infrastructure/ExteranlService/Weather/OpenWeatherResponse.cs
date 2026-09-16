using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AITravelB.Infrastructure.ExteranlService.Weather
{
    public class OpenWeatherResponse
    {
        [JsonPropertyName("list")]
        public List<OpenWeatherListItem> List { get; set; } = new();
    }
    public class OpenWeatherListItem
    {
        [JsonPropertyName("dt_txt")]
        public string DtTxt { get; set; } = string.Empty;

        [JsonPropertyName("main")]
        public OpenWeatherMain Main { get; set; } = new();

        [JsonPropertyName("weather")]
        public List<OpenWeatherCondition> Weather { get; set; } = new();

        [JsonPropertyName("pop")]
        public double Pop { get; set; } // from  0 to 1
    }

    public class OpenWeatherMain
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }
    }
        public class OpenWeatherCondition
    {
        [JsonPropertyName("main")]
        public string Main { get; set; } = string.Empty;
    }
}
