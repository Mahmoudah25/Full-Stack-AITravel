using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task NotificationBopkingStatusChangedAsync(string customerEmail, Guid bookingId, string newStatus);  
    }
}
