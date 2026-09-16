using AITravelB.Application.Trips.Queries.GrtTripById;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Trips.Queries.GetTripByName
{
    public class GetTripByNameQuery : IRequest<TripDto?>
    {
        public string Name { get; set; }
        public GetTripByNameQuery(string name)
        {
            Name = name;
        }
    }
    
}
