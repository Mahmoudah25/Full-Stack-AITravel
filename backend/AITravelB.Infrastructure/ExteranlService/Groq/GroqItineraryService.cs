using AITravelB.Application.Common.DTOs;
using AITravelB.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AITravelB.Infrastructure.ExteranlService.Groq
{
    public class GroqItineraryService : IItineraryAiService
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        public GroqItineraryService(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
        }
        public async Task<ItineraryResult> GenerateItineraryAsync(string destination, int days, decimal budget, List<WeatherForecastDto>? weatherForecast = null,List<PlaceDto>? availablePlaces = null)
        {
            var apiKey = configuration["ExternalServices:AiProvider:ApiKey"]
               ?? throw new InvalidOperationException("API key is missing.");
            var prompt = GenerateGroqQuery(destination, days, budget,weatherForecast,availablePlaces);
            var requestBody = BuildRequestBody(prompt);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Groq API error ({response.StatusCode}): {errorBody}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var groqResponse = JsonSerializer.Deserialize<GroqResponse>(responseContent);

            string? rawJsonText = groqResponse?.Choices?.FirstOrDefault()?.Message?.Content;

            if (string.IsNullOrWhiteSpace(rawJsonText))
                throw new InvalidOperationException("Groq returned an empty response.");

            var itinerary = JsonSerializer.Deserialize<ItineraryResult>(rawJsonText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return itinerary ?? throw new InvalidOperationException("Failed to parse itinerary result.");
        }

 
        private string GenerateGroqQuery(string destination, int days, decimal budget,List<WeatherForecastDto>? weatherForecast , List<PlaceDto>? availablePlaces = null)
        {
            string weatherNote = BuildWeatherNote(weatherForecast); // You can pass actual weatherForecast if available
            string placeNote = buildplaceNote(availablePlaces); // You can pass actual availablePlaces if available
            return $@"You are a professional travel planner. Create a {days}-day travel itinerary for {destination} 
             with a total budget of {budget} USD. Distribute activities logically by time of day.
               {weatherNote}
               {placeNote}
              Return ONLY valid JSON in exactly this structure, with no extra text:
             {{
             ""days"": [
              {{
              ""dayNumber"": 1,
              ""activities"": [
             {{ ""time"": ""09:00"", ""placeName"": ""Example Place"", ""type"": ""attraction"", ""estimatedCost"": 20.0 }}
             ]
             }}
            ],
            ""totalEstimatedCost"": 0
            }}";
        }

        private string buildplaceNote(List<PlaceDto>? availablePlaces)
        {
            if(availablePlaces == null || availablePlaces.Count == 0)
                return string.Empty;
            var sb = new StringBuilder();
            sb.AppendLine("Here are real places available in this destination. You MUST choose activity names ONLY from this list (use the exact name):");
            foreach (var place in availablePlaces)
            {
                sb.AppendLine($"- {place.Name}");
            }
            return sb.ToString();
        }
        private string BuildWeatherNote(List<WeatherForecastDto> weatherForecast)
        {
            if (weatherForecast == null || weatherForecast.Count == 0)
                return string.Empty;
            var sb = new StringBuilder();
            sb.AppendLine("Consider the following weather forecasts for your trip:");
            foreach (var day in weatherForecast)
            {
                bool highrainChance = day.RainProbability >= 0.5;
                sb.AppendLine($"- {day.Date:yyyy-MM-dd}: {day.Condition}, {day.TemperatureCelsius:F1}°C, rain chance {day.RainProbability:P0}" +
                         (highrainChance ? " → prefer INDOOR activities this day." : ""));
            }
            return sb.ToString();
        }
        private object BuildRequestBody(string prompt)
        {
            return new
            {
                model = "openai/gpt-oss-120b",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful travel planning assistant that always responds with valid JSON only." },
                    new { role = "user", content = prompt }
                },
                response_format = new { type = "json_object" },
                max_tokens = 4000
            };
        }

       
    }
}
