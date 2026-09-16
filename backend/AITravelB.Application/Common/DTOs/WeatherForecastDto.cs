using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Common.DTOs
{
    public class WeatherForecastDto
    {
        public DateTime Date {  get; set; }
        public double TemperatureCelsius { get; set; }
        public string Condition { get; set; } = string.Empty;
        public double RainProbability { get; set; } 
    }
}
