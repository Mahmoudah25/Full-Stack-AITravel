using AITravelB.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Trips.Queries.GrtTripById
{
    public class GetTripByIdQueryHandler : IRequestHandler<GetTripByIdQuery, TripDto?>
    {
        private readonly IApplicationDbContext context;
        public GetTripByIdQueryHandler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<TripDto?> Handle(GetTripByIdQuery request, CancellationToken cancellationToken)
        {
            var trip = await context.Trips.FirstOrDefaultAsync(t => t.Id == request.Id);
            if(trip == null)
            {
                return null;
            }
            return new TripDto
            {
                Id = trip.Id,
                Destination = trip.Destination,
                StartDate = trip.StartDate,
                Days = trip.Days,
                BudgetAmount = trip.Budget.Amount,
                BudgetCurrency = trip.Budget.Currency
            };

        }
    }
}
