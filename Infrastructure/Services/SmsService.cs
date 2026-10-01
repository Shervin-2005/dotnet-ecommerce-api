using Application.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        private const string SendUrl = "https://api.sms.ir/v1/send/verify";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SmsSettings _smsSettings;

        public SmsService(IHttpClientFactory httpClientFactory, IOptions<SmsSettings> smsSettings)
        {
            _httpClientFactory= httpClientFactory;
            _smsSettings = smsSettings.Value;
        }

        public async Task SendAsync(string phoneNumber, string code)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", _smsSettings.OtpApiKey);

            var model = new
            {
                Mobile = phoneNumber,
                TemplateId = _smsSettings.OtpTemplateId,
                Parameters = new[]
                {
                    new { Name = "Code", Value = code }
                }
            };

            var payload = JsonSerializer.Serialize(model);
            StringContent content = new(payload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(
                SendUrl,
                content
            );

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"SMS sending failed. Status: {response.StatusCode}");
        }
    }
}
