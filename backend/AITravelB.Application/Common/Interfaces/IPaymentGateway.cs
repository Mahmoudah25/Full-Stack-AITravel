using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Common.Interfaces
{
    public interface IPaymentGateway
    {
        Task<string> GetAuthTokenAsync();
        Task<string> CreateOrderAsync(string authToken, decimal amount, string currency, string orderId);
        Task<string> GetPaymentKeyAsync(string authToken, string paymobOrderId,
                                        decimal amount, string currency, string billingEmail);
        bool VerifyHmac(Dictionary<string, string> callbackData, string receivedHmac);
        string IFrameId { get; }
    }
}
