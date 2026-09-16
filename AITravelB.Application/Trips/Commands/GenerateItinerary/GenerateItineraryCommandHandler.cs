using AITravelB.Application.Common.DTOs;
using AITravelB.Application.Common.Interfaces;
using AITravelB.Domain.Entities;
using AITravelB.Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AITravelB.Application.Trips.Commands.GenerateItinerary
{
    public class GenerateItineraryCommandHandler : IRequestHandler<GenerateItinerary, ItineraryResult>
    {
        private readonly IItineraryAiService apiservice;
        private readonly IWeatherService weatherService;
        private readonly IApplicationDbContext context;
        private readonly IPlacesService placesService;

        public GenerateItineraryCommandHandler(
            IItineraryAiService apiservice,
            IWeatherService weatherService,
            IApplicationDbContext context,
            IPlacesService placesService)
        {
            this.apiservice = apiservice;
            this.weatherService = weatherService;
            this.context = context;
            this.placesService = placesService;
        }

        public async Task<ItineraryResult> Handle(GenerateItinerary request, CancellationToken cancellationToken)
        {
            // 1. جيب الطقس (لو فشل، نكمل من غيره)
            List<WeatherForecastDto>? weatherForecast = null;
            try
            {
                weatherForecast = await weatherService.GetForecastAsync(request.Destination, request.Days);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Weather service failed: {ex.Message}");
            }
            
            // 2. جيب الأماكن الحقيقية (لو فشلت، نكمل من غيرها)
            List<PlaceDto> availablePlaces = new();
            try
            {
                var restaurants = await placesService.SearchPlacesAsync(request.Destination, "restaurant");
                var attractions = await placesService.SearchPlacesAsync(request.Destination, "attraction");
                availablePlaces = restaurants.Concat(attractions).ToList();
                Console.WriteLine($"Found {availablePlaces.Count} places from OSM for {request.Destination}");
                foreach (var place in availablePlaces.Take(5))
                {
                    Console.WriteLine($"  - {place.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Places service failed: {ex.Message}");
            }

            // 3. تأكد إن الرحلة موجودة (مرة واحدة بس)
            var tripExists = await context.Trips.AnyAsync(t => t.Id == request.TripId, cancellationToken);
            if (!tripExists)
                throw new InvalidOperationException($"Trip with ID {request.TripId} not found.");

            // 4. نادِ الـ AI بكل المعلومات مع بعض
            var itinerary = await apiservice.GenerateItineraryAsync(
                request.Destination, request.Days, request.Budget, weatherForecast, availablePlaces);

            if (weatherForecast != null)
            {
                foreach (var day in itinerary.Days)
                {
                    var marchingWeather = weatherForecast.ElementAtOrDefault(day.DayNumber - 1);
                    if (marchingWeather != null)
                    {
                        day.WeatherCondition = marchingWeather.Condition;
                        day.TemperatureCelsius = marchingWeather.TemperatureCelsius;

                    }
                }
            }

            // 5. احفظ كل نشاط
            foreach (var day in itinerary.Days)
            {
                foreach (var activityPlan in day.Activities)
                {
                    var mappedType = MapToActivityType(activityPlan.Type);
                    var newActivity = new Activity(
                        activityPlan.PlaceName,
                        mappedType,
                        activityPlan.EstimatedCost,
                        activityPlan.Time,
                        "ai-generated",
                        request.TripId);

                    context.Activities.Add(newActivity);
                    activityPlan.ActivityId = newActivity.Id;
                }
            }

            await context.SaveChangesAsync(cancellationToken);
            return itinerary;
        }

        private ActivityType MapToActivityType(string rawType)
        {
            return rawType.ToLower() switch
            {
                "food" or "meal" or "restaurant" => ActivityType.Restaurant,
                "hotel" => ActivityType.Hotel,
                _ => ActivityType.Attraction
            };
        }
    }
}