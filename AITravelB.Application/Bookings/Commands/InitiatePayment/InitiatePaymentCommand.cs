using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Bookings.Commands.InitiatePayment
{
    public class InitiatePaymentCommand:IRequest<string>
    {
        public Guid BookingId { get; set; }
    }
}
