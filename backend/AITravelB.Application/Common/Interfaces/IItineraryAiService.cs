using AITravelB.Application.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Common.Interfaces
{
    public interface IItineraryAiService
    {
        Task<ItineraryResult> GenerateItineraryAsync (string destination, int days, decimal budget,List<WeatherForecastDto>? weatherForecasts, List<PlaceDto>? availablePlaces =null);
    }
}
