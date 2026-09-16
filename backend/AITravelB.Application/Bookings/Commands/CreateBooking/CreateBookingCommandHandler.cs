using AITravelB.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommandHandler:IRequestHandler<CreateBookingCommand, Guid>
    {
        private readonly IApplicationDbContext context;
        public CreateBookingCommandHandler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = new Domain.Entities.Booking
            (   request.TripId,
                request.ActivityId,
                request.Amount,
                "EGP",
                request.CustomerEmail
            );
            await context.Bookings.AddAsync(booking);
            await context.SaveChangesAsync(cancellationToken);
            return booking.Id;
        }
    }
}
