using Application.Models;
using Application.Services;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services
{
    public class PaymentService(PaymentProviderOptions options) : IPaymentService
    {
        private sealed class CreateOrderResponse
        {
            [JsonPropertyName("order")]
            public OrderDto Order { get; set; } = default!;
        }

        private sealed class OrderDto
        {
            [JsonPropertyName("id")] public long Id { get; set; }
            [JsonPropertyName("hppUrl")] public string HppUrl { get; set; } = "";
            [JsonPropertyName("password")] public string Password { get; set; } = "";
            [JsonPropertyName("status")] public string Status { get; set; } = "";
            [JsonPropertyName("cvv2AuthStatus")] public string Cvv2AuthStatus { get; set; } = "";
            [JsonPropertyName("secret")] public string Secret { get; set; } = "";
        }

        public async Task<string> CreatePaymentOrder()
        {
            var credentials = Encoding.ASCII.GetBytes($"{options.Username}:{options.Password}");
            var base64Credentials = Convert.ToBase64String(credentials);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Credentials);
            client.BaseAddress = new Uri(options.BaseUrl);

            var body = new
            {
                order = new
                {
                    typeRid = "Order_SMS",
                    amount = "0.01",
                    currency = "AZN",
                    language = "az",
                    description = "Testdesc",
                    hppRedirectUrl = $"{options.CallbackUrl}/Payment/callback",
                    hppCofCapturePurposes = new[] { "Cit" }
                }
            };

            var response = await client.PostAsJsonAsync("/api/order", body);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to create payment order: {response.ReasonPhrase}");
            }
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var payload = await response.Content.ReadFromJsonAsync<CreateOrderResponse>(jsonOptions) ?? throw new InvalidOperationException($"Failed to create payment order. try again");

            return $"{payload.Order.HppUrl}?id={payload.Order?.Id}&password={payload.Order?.Password}" ?? throw new InvalidOperationException("Failed to retrieve payment URL from response.");
            
        }

        public Task<string> CreatePaymentTransaction(string orderId, string status)
        {
            throw new NotImplementedException();
        }
    }
}
