using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GCI_Admin.Models;
using Microsoft.Extensions.Options;
using Utils;

namespace Utils
{
    public class CommunicationService
    {
        private readonly SmsConfig _smsConfig;
        private readonly EmailConfig _emailConfig;
        private readonly HttpClient _httpClient;

        public CommunicationService(
            IOptions<SmsConfig> smsConfig,
            IOptions<EmailConfig> emailConfig,
            HttpClient httpClient)
        {
            _smsConfig = smsConfig?.Value ?? throw new ArgumentNullException(nameof(smsConfig));
            _emailConfig = emailConfig?.Value ?? throw new ArgumentNullException(nameof(emailConfig));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> SendSmsAsync(string mobile, string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_smsConfig.BaseUrl))
                    throw new InvalidOperationException("SMS BaseUrl is not configured.");

                if (string.IsNullOrWhiteSpace(mobile))
                    throw new ArgumentException("Mobile number is required.", nameof(mobile));

                if (string.IsNullOrWhiteSpace(message))
                    throw new ArgumentException("Message cannot be empty.", nameof(message));

                var payload = new
                {
                    partnerID = _smsConfig.PartnerId,
                    apikey = _smsConfig.ApiKey,
                    mobile = mobile,
                    message = message,
                    shortcode = _smsConfig.Shortcode,
                    pass_type = "plain"
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_smsConfig.BaseUrl, content);

                var responseContent = await response.Content.ReadAsStringAsync();
                Loggers.EventLogs($"SMS send attempt to {mobile}. Status: {(int)response.StatusCode}, Response: {responseContent}");
                if (!response.IsSuccessStatusCode)
                {
                    Loggers.DoLogs($"SMS sending failed. Status: {(int)response.StatusCode}, Response: {responseContent}");
                }
                else
                {
                    Loggers.EventLogs($"SMS sent successfully to {mobile}. Response: {responseContent}");
                }

                return responseContent;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"SendSmsAsync Error: {ex}");
                return $"Error sending SMS: {ex.Message}";
            }
        }

        public async Task<string> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_emailConfig.BaseUrl))
                    throw new InvalidOperationException("Email BaseUrl is not configured.");

                if (string.IsNullOrWhiteSpace(to))
                    throw new ArgumentException("Recipient email is required.", nameof(to));

                var payload = new
                {
                    to = to,
                    subject = subject,
                    body = body,
                    from = _emailConfig.FromEmail
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                Loggers.EventLogs($"Sending Email to {to}: {subject}");

                var response = await _httpClient.PostAsync(_emailConfig.BaseUrl, content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Loggers.DoLogs($"Email failed. Status: {(int)response.StatusCode}, Response: {result}");
                }

                return result;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"SendEmailAsync Error: {ex}");
                return $"Error sending Email: {ex.Message}";
            }
        }
    }
}