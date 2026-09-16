using AITravelB.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AITravelB.API.Controllers
{
    [Route("api/webhooks")]
    [ApiController]
    public class PaymobWebhookController : ControllerBase
    {
        private readonly IApplicationDbContext context;
        private readonly IPaymentGateway paymentGateway;
        private readonly INotificationService notificationService;
        public PaymobWebhookController(IApplicationDbContext context, IPaymentGateway paymentGateway, INotificationService notificationService)
        {
            this.context = context;
            this.paymentGateway = paymentGateway;
            this.notificationService = notificationService;
        }

        [HttpPost("paymob")]
        public async Task<IActionResult> HandlePaymobCallback([FromQuery(Name = "hmac")] string receivedHmac)
        {
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();
            var json = JsonDocument.Parse(rawBody);
            var obj = json.RootElement.GetProperty("obj");
            var callbackData = new Dictionary<string, string>()
            {
                ["amount_cents"] = obj.GetProperty("amount_cents").ToString(),
                ["created_at"] = obj.GetProperty("created_at").GetString() ?? "",
                ["currency"] = obj.GetProperty("currency").GetString() ?? "",
                ["error_occured"] = obj.GetProperty("error_occured").ToString().ToLower(),
                ["has_parent_transaction"] = obj.GetProperty("has_parent_transaction").ToString().ToLower(),
                ["id"] = obj.GetProperty("id").ToString(),
                ["integration_id"] = obj.GetProperty("integration_id").ToString(),
                ["is_3d_secure"] = obj.GetProperty("is_3d_secure").ToString().ToLower(),
                ["is_auth"] = obj.GetProperty("is_auth").ToString().ToLower(),
                ["is_capture"] = obj.GetProperty("is_capture").ToString().ToLower(),
                ["is_refunded"] = obj.GetProperty("is_refunded").ToString().ToLower(),
                ["is_standalone_payment"] = obj.GetProperty("is_standalone_payment").ToString().ToLower(),
                ["is_voided"] = obj.GetProperty("is_voided").ToString().ToLower(),
                ["order"] = obj.GetProperty("order").GetProperty("id").ToString(),
                ["owner"] = obj.GetProperty("owner").ToString(),
                ["pending"] = obj.GetProperty("pending").ToString().ToLower(),
                ["source_data_pan"] = obj.GetProperty("source_data").GetProperty("pan").GetString() ?? "",
                ["source_data_sub_type"] = obj.GetProperty("source_data").GetProperty("sub_type").GetString() ?? "",
                ["source_data_type"] = obj.GetProperty("source_data").GetProperty("type").GetString() ?? "",
                ["success"] = obj.GetProperty("success").ToString().ToLower()
            };
            bool isValidHmac = paymentGateway.VerifyHmac(callbackData, receivedHmac);
            if (!isValidHmac)
                return Unauthorized(" Invalid Hmac signature");
            var success = obj.GetProperty("success").GetBoolean();
            string merchantOrderId = obj.GetProperty("order").GetProperty("merchant_order_id").ToString();
            if (!Guid.TryParse(merchantOrderId,out var bookingId))
                return BadRequest("Invalid booking ID in the callback data.");
            var booking = await context.Bookings.FindAsync(bookingId);
            if (booking == null)
                return NotFound("Booking not found.");
            if(success)
                booking.MarkAsPaid(obj.GetProperty("order").GetProperty("id").ToString());
            else
                booking.MarkAsFailed();
            await context.SaveChangesAsync(default);
            await notificationService.NotificationBopkingStatusChangedAsync(
                booking.CustomerEmail,
                booking.Id,
                booking.Status.ToString());
            return Ok();
        }
    }
}
