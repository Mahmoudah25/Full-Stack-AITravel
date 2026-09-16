using AITravelB.Application.Common.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Trips.Commands.GenerateItinerary
{
    public class GenerateItinerary:IRequest<ItineraryResult>
    {
        public Guid TripId { get; set; }
        public string Destination { get; set; } = string.Empty;
        public int Days { get; set; }
        public decimal Budget { get; set; }
    }
}
