using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Common.DTOs
{
    public class PlaceDto
    {
        public string Name { get; set; } = string.Empty;
        public double Rating { get; set; }
        public double Latitude { get; set; } 
        public double Longitude { get; set; } 
        public string PhotoUrl { get; set; } = string.Empty;
        public string PlaceId { get; set; } = string.Empty;
    }


}
