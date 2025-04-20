using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using PayPal.Application.DTOs;
using PayPal.Application.services;

namespace PayPal.Application.Services
{
    public class PayPalService : IPayPalService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public PayPalService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<PayoutResponseDto> SendPayoutAsync(PayoutRequestDto request)
        {
            try
            {
                var token = await GetAccessTokenAsync();

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                request.SenderBatchHeader.SenderBatchId ??= $"Payouts_{DateTime.UtcNow:yyyyMMdd_HHmmss}";

                for (int i = 0; i < request.Items.Count; i++)
                {
                    request.Items[i].SenderItemId ??= $"{request.SenderBatchHeader.SenderBatchId}_{i + 1}";
                }

                var url = "https://api.sandbox.paypal.com/v1/payments/payouts";

                var jsonContent = JsonConvert.SerializeObject(request);

                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, httpContent);


                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    throw new HttpRequestException($"PayPal API error: {response.StatusCode} - {errorContent}");
                }


                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<PayoutResponseDto>(responseContent);

            }
            catch (Exception ex)
            {
              
                throw new ApplicationException("Failed to process PayPal payout", ex);
              
            }
          
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var clientId = _config["PayPal:ClientId"];

            var secret = _config["PayPal:Secret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(secret))
            {
                throw new InvalidOperationException("PayPal credentials not configured");
            }

            var byteArray = Encoding.ASCII.GetBytes($"{clientId}:{secret}");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-m.sandbox.paypal.com/v1/oauth2/token");

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException($"PayPal auth error: {response.StatusCode} - {errorContent}");
            }

            var json = await response.Content.ReadAsStringAsync();

            dynamic result = JsonConvert.DeserializeObject(json);

            if (result == null || string.IsNullOrEmpty(result.access_token.ToString()))
            {
                throw new InvalidOperationException("Failed to retrieve PayPal access token");
            }

            return result.access_token;

        }
    }
}
