using AITravelB.Application.Common.Interfaces;
using AITravelB.Application.Trips.Queries.GrtTripById;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Trips.Queries.GetTripByName
{
    public class GetTripByNameQueryHandler : IRequestHandler<GetTripByNameQuery, TripDto?>
    {
        private readonly IApplicationDbContext context;
        public GetTripByNameQueryHandler(IApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<TripDto?> Handle(GetTripByNameQuery request, CancellationToken cancellationToken)
        {
            var trip = await context.Trips.FirstOrDefaultAsync(t => t.Destination == request.Name);
            if (trip == null)
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
