using AITravelB.API.Hubs;
using AITravelB.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AITravelB.API.Services
{
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> hubContext;
        public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
        {
            this.hubContext = hubContext;
        }
        public async Task NotificationBopkingStatusChangedAsync(string customerEmail, Guid bookingId, string newStatus)
        {
            await hubContext.Clients.Group(customerEmail).SendAsync("BookingStatusChanged", new { BookingId = bookingId, NewStatus = newStatus });  
        }
    }
}
