using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommand :IRequest<Guid>
    {
        public Guid TripId { get; set; }
        public Guid ActivityId { get; set; }
        public decimal Amount { get; set; }
        public string Currency {  get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
    }
}
