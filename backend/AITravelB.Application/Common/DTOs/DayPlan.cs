using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Common.DTOs
{
    public class DayPlan
    {
        public int DayNumber {  get; set; }
        public List<ActivityPlan> Activities { get; set; } = new();
        public string? WeatherCondition { get; set; }
        public double TemperatureCelsius { get; set; }

    }
}
