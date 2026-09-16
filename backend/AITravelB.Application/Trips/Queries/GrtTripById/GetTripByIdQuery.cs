using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Trips.Queries.GrtTripById
{
    public class GetTripByIdQuery : IRequest<TripDto?>
    {
        public Guid Id { get; set; }
        public GetTripByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
