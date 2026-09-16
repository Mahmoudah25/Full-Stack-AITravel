using AITravelB.Application.Common.Interfaces;
using AITravelB.Application.Common.setting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AITravelB.Infrastructure.ExteranlService.Paymob
{
    public class PayMobService : IPaymentGateway
    {
        private readonly HttpClient httpClient;
        private readonly PayMobSetting payMobSetting;
        public PayMobService(HttpClient httpClient, IOptions<PayMobSetting> options)
        {
            this.httpClient =  httpClient;
            this.payMobSetting = options.Value;
        }
        public string IFrameId => payMobSetting.IFrameId;

        public async Task<string> CreateOrderAsync(string authToken, decimal amount, string currency, string orderId)
        {
            var body = new
            {
                auth_token = authToken,
                delivery_needed = false,
                merchant_order_id = orderId,
                currency = currency,
                items = Array.Empty<object>(),
                amount_cents = (int)(amount * 100)
            };
            var response = await httpClient.PostAsJsonAsync("ecommerce/orders", body);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Paymob error ({response.StatusCode}): {errorBody}");
            }
            var res = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (res.TryGetProperty("id", out var idProperty))
            {
                return idProperty.GetInt32().ToString();
            }
            else
            {
                throw new InvalidOperationException("Order ID not found in the response.");
            }
        }

        public async Task<string> GetAuthTokenAsync()
        {
            var body = new
            {
                api_key = payMobSetting.ApiKey
            };
            var response = await httpClient.PostAsJsonAsync("auth/tokens", body);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Paymob error ({response.StatusCode}): {errorBody}");
            }
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (result.TryGetProperty("token", out var tokenProperty))
                return tokenProperty.GetString()!;
            throw new InvalidOperationException("Auth token not found in the response.");
        }

        public async Task<string> GetPaymentKeyAsync(string authToken, string paymobOrderId, decimal amount, string currency, string billingEmail)
        {
            var body = new
            {
                auth_token = authToken,
                amount_cents = (int)(amount * 100),
                expiration = 3600,
                order_id = paymobOrderId,
                billing_data = new
                {
                    email = billingEmail,
                    first_name = "N/A",
                    last_name = "N/A",
                    phone_number = "N/A",
                    apartment = "N/A",
                    floor = "N/A",
                    street = "N/A",
                    building = "N/A",
                    shipping_method = "N/A",
                    postal_code = "N/A",
                    city = "N/A",
                    country = "N/A",
                    state = "N/A"
                },
                currency = currency,
                integration_id = payMobSetting.IntegrationId
            };
            var response = await httpClient.PostAsJsonAsync("acceptance/payment_keys", body); // relative
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            return result.GetProperty("token").GetString()!;
        }

        public bool VerifyHmac(Dictionary<string, string> callbackData, string receivedHmac)
        {
            var fields = new[]
            {
                "amount_cents", "created_at", "currency", "error_occured",
                "has_parent_transaction", "id", "integration_id", "is_3d_secure",
                "is_auth", "is_capture", "is_refunded", "is_standalone_payment",
                "is_voided", "order", "owner", "pending",
                "source_data_pan", "source_data_sub_type", "source_data_type",
                "success"
            };

            var concatenatedString = string.Join("", fields.Select(field => callbackData.ContainsKey(field) ? callbackData[field] : ""));
            using var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(payMobSetting.HmacSecret)); 
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString));
            var computedHmac = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            return computedHmac == receivedHmac.ToLowerInvariant();
        }
    }
}
