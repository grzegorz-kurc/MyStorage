using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MyStorageAPI.Models.Configuration;
using MyStorageAPI.Models.Responses;
using MyStorageAPI.Services.Interfaces;

namespace MyStorageAPI.Services
{
	public class EmailService : IEmailService
	{
		private readonly HttpClient _httpClient;
		private readonly EmailServiceConfig _config;
		private readonly ILogger<EmailService> _logger;

		public EmailService(HttpClient httpClient, IOptions<AppConfig> config, ILogger<EmailService> logger)
		{
			_httpClient = httpClient;
			_config = config.Value.EmailService;
			_logger = logger;
		}

		public async Task<SendEmailResult> SendEmailAsync(string to, string subject, string body)
		{
			if (string.IsNullOrEmpty(_config.ConsumerKey) || string.IsNullOrEmpty(_config.ConsumerSecret))
			{
				_logger.LogError("Email service credentials are missing in configuration.");
				return new SendEmailResult { Success = false, Errors = new List<string> { "Email service configuration is missing." } };
			}

			var url = _config.BaseUrl;

			var mailData = new
			{
				from = _config.SenderEmail,
				to,
				subject,
				content = body,
				html_content = $"<p>{body}</p>"
			};

			try
			{
				var json = JsonSerializer.Serialize(mailData);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				// Set authentication headers
				content.Headers.Add("consumerKey", _config.ConsumerKey);
				content.Headers.Add("consumerSecret", _config.ConsumerSecret);

				var response = await _httpClient.PostAsync(url, content);

				if (!response.IsSuccessStatusCode)
				{
					var errorResponse = await response.Content.ReadAsStringAsync();
					_logger.LogError($"Email sending failed: {errorResponse}");
					return new SendEmailResult { Success = false, Errors = new List<string> { "Failed to send email." } };
				}

				return new SendEmailResult { Success = true };
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception occurred while sending email.");
				return new SendEmailResult { Success = false, Errors = new List<string> { "Error sending email. Try again later." } };
			}
		}
	}
}