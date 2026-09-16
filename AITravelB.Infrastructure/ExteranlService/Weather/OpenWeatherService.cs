using AITravelB.Application.Common.DTOs;
using AITravelB.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Infrastructure.ExteranlService.Weather
{
    public class OpenWeatherService : IWeatherService
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        public OpenWeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
        }
        public async Task<List<WeatherForecastDto>> GetForecastAsync(string city, int days)
        {
            var apiKey = configuration["ExternalServices:OpenWeather:ApiKey"]
                ?? throw new InvalidOperationException("OpenWeather API key is missing.");
            var url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apiKey}&units=metric";
            var resquest = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await httpClient.SendAsync(resquest);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var weatherReponse = System.Text.Json.JsonSerializer.Deserialize<OpenWeatherResponse>(json);
            var gruoped = weatherReponse?.List
                .GroupBy(x => DateTime.Parse(x.DtTxt).Date)
                .Take(days);
            var result = new List<WeatherForecastDto>();
            foreach (var group in gruoped ?? Enumerable.Empty<IGrouping<DateTime, OpenWeatherListItem>>())
            {
                var avgerageTemp = group.Average(x => x.Main.Temp);
                var MaxTemp = group.Max(x => x.Main.Temp);
                var firstWeather = group.FirstOrDefault()?.Weather.FirstOrDefault();

                var newresult = new WeatherForecastDto
                {
                    Date = group.Key,
                    TemperatureCelsius = avgerageTemp,
                    Condition = firstWeather?.Main ?? "Unknown",
                    RainProbability = MaxTemp,
                };
                result.Add(newresult);

            }

            return result;
        }
    }
}
