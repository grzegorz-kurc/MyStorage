namespace MyStorageAPI.Models.Configuration
{
	public class AppConfig
	{
		public string BaseUrl { get; set; } = string.Empty;
		public EmailServiceConfig EmailService { get; set; } = new();
		public JwtSettings Jwt { get; set; } = new();
	}

	public class EmailServiceConfig
	{
		public string ConsumerKey { get; set; } = string.Empty;
		public string ConsumerSecret { get; set; } = string.Empty;
		public string SenderEmail { get; set; } = string.Empty;
		public string SenderName { get; set; } = string.Empty;
		public string BaseUrl { get; set; } = string.Empty;
	}

	public class JwtSettings
	{
		public string Issuer { get; set; } = string.Empty;
		public string Audience { get; set; } = string.Empty;
		public string SecretKey { get; set; } = string.Empty;
		public int TokenLifetimeMinutes { get; set; }
	}
}