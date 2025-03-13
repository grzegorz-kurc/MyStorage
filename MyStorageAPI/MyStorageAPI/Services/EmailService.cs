using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MyStorageAPI.Models.Configuration;
using MyStorageAPI.Services.Interfaces;

namespace MyStorageAPI.Services
{
	public class EmailService : IEmailService
	{
		private readonly HttpClient _httpClient;
		private readonly EmailServiceConfig _config;

		public EmailService(HttpClient httpClient, IOptions<AppConfig> config)
		{
			_httpClient = httpClient;
			_config = config.Value.EmailService;
		}

		public async Task<bool> SendEmailAsync(string to, string subject, string body)
		{
			if (string.IsNullOrEmpty(_config.ConsumerKey) || string.IsNullOrEmpty(_config.ConsumerSecret))
				throw new InvalidOperationException("TurboSMTP credentials are missing in configuration.");

			var url = _config.BaseUrl;

			var mailData = new
			{
				from = _config.SenderEmail,
				to,
				subject,
				content = body,
				html_content = $"<p>{body}</p>"
			};

			var json = JsonSerializer.Serialize(mailData);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			// Set authentication headers
			content.Headers.Add("consumerKey", _config.ConsumerKey);
			content.Headers.Add("consumerSecret", _config.ConsumerSecret);

			var response = await _httpClient.PostAsync(url, content);
			return response.IsSuccessStatusCode;
		}
	}
}