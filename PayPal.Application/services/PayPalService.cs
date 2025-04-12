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

                var senderBatchId = string.IsNullOrEmpty(request.SenderBatchHeader.SenderBatchId)? "Payouts_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"): request.SenderBatchHeader.SenderBatchId;
                
                request.SenderBatchHeader.SenderBatchId = senderBatchId;

                for (int i = 0; i < request.Items.Count; i++)
                {
                    if (string.IsNullOrEmpty(request.Items[i].SenderItemId))
                    {
                        request.Items[i].SenderItemId = $"{senderBatchId}_{i + 1}";
                    }
                }

                var jsonContent = JsonConvert.SerializeObject(request);

                var response = await _httpClient.PostAsync("/v1/payments/payouts",new StringContent(jsonContent, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    throw new HttpRequestException($"PayPal API error: {response.StatusCode} - {errorContent}");
                }

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
