using AITravelB.Application.Common.Interfaces;
using AITravelB.Domain.Entities;
using AITravelB.Domain.ValueObject;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Trips.Commands.Createtrip
{
    public class CreateTripCommandHandler : IRequestHandler<CreateTripCommand, Guid>
    {
        private readonly IApplicationDbContext context;
        public CreateTripCommandHandler(IApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<Guid> Handle(CreateTripCommand request, CancellationToken cancellationToken)
        {
            var budget = new Budget(request.BudgetAmount,request.BudgetCurrency);
            var trip = new Trip(request.Destination, request.StartDate, request.Days, budget);
            context.Trips.Add(trip);
            await context.SaveChangesAsync(cancellationToken);
            return trip.Id;
        }
    }
}
