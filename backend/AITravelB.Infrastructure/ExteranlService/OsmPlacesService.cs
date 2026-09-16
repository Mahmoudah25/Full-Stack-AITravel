using AITravelB.Application.Common.DTOs;
using AITravelB.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Infrastructure.Service
{
    public class OsmPlacesService : IPlacesService
    {
        private readonly HttpClient httpClient;
        public OsmPlacesService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<List<PlaceDto>> SearchPlacesAsync(string city, string category)
        {
            var (tagKey, tagValue) = MapCategoryToOsmTag(category);
            string query = $@"
               [out:json][timeout:25];
              area[""name""=""{city}""]->.searchArea;
                 (
                 node[""{tagKey}""=""{tagValue}""](area.searchArea);
                  );
                 out body 20;";

            var content = new StringContent(query, Encoding.UTF8, "text/plain");
            var response = await httpClient.PostAsync("https://overpass.kumi.systems/api/interpreter", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var overpassResponse = System.Text.Json.JsonSerializer.Deserialize<OverpassResponse>(json);
            return MapToPlaceDtos(overpassResponse);

        }

        private (string Key, string Value) MapCategoryToOsmTag(string category) => category.ToLower() switch
        {
            "restaurant" => ("amenity", "restaurant"),
            "hotel" => ("tourism", "hotel"),
            "attraction" => ("tourism", "attraction"),
            _ => ("amenity", "restaurant")
        }; 

        private List<PlaceDto> MapToPlaceDtos(OverpassResponse? result)
        {
            if(result == null || result.Elements == null)
            {
                return new List<PlaceDto>();
            }

            return result.Elements.Select(e => new PlaceDto
            {
                Name = e.Tags != null && e.Tags.ContainsKey("name") ? e.Tags["name"] : "Unknown",
                Latitude = e.Lat,
                Longitude = e.Lon,
                PlaceId = e.Id.ToString(),
                PhotoUrl = "", // OSM does not provide photos directly
                Rating = 0 // OSM does not provide ratings
            }).ToList();

        }
    }
}
