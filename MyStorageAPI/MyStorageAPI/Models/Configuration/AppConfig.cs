namespace MyStorageAPI.Models.Configuration
{
	public class AppConfig
	{
		public string ClientBaseUrl { get; set; } = string.Empty; // TODO: Remove?
		public EmailServiceConfig EmailService { get; set; } = new EmailServiceConfig();
	}

	public class EmailServiceConfig
	{
		public string ConsumerKey { get; set; } = string.Empty;
		public string ConsumerSecret { get; set; } = string.Empty;
		public string SenderEmail { get; set; } = string.Empty;
		public string SenderName { get; set; } = string.Empty;
		public string BaseUrl { get; set; } = string.Empty;
	}
}