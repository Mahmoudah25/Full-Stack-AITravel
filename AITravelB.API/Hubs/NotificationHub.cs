using Microsoft.AspNetCore.SignalR;

namespace AITravelB.API.Hubs
{
    public class NotificationHub:Hub
    {
       public async Task SendNotification(string CustomerEmail)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, CustomerEmail);
        }
    }
}
