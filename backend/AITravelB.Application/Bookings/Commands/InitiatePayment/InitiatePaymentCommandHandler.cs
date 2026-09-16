using AITravelB.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Bookings.Commands.InitiatePayment
{
    public class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, string>
    {
        private readonly IPaymentGateway paymentGateway;
        private readonly IApplicationDbContext context;
        public InitiatePaymentCommandHandler(IPaymentGateway paymentGateway, IApplicationDbContext context)
        {
            this.paymentGateway = paymentGateway;
            this.context = context;
        }
        public async Task<string> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
        {
            var booking = await context.Bookings.FirstOrDefaultAsync(b => b.Id == request.BookingId);
            if (booking == null)
                throw new InvalidOperationException("Booking not found.");
            if(booking.Status != Domain.Enum.BookingStatus.Pending)
                throw new InvalidOperationException("Booking is not in a valid state for payment initiation.");
            var authToken =  await paymentGateway.GetAuthTokenAsync();
            var paymobOrderId = await paymentGateway.CreateOrderAsync(authToken,booking.Amount,booking.Currency,booking.Id.ToString());
            var paymentKey = await paymentGateway.GetPaymentKeyAsync(authToken, paymobOrderId, booking.Amount,booking.Currency, booking.CustomerEmail);
            string iframeUrl = $"https://accept.paymob.com/api/acceptance/iframes/{paymentGateway.IFrameId}?payment_token={paymentKey}";
            return iframeUrl;

        }
    }
}
