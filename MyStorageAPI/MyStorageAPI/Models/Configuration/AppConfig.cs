namespace MyStorageAPI.Models.Configuration
{
	public class AppConfig
	{
		public EmailServiceConfig EmailService { get; set; } = new EmailServiceConfig();
	}

	public class EmailServiceConfig
	{
		public string ConsumerKey { get; set; } = string.Empty;
		public string ConsumerSecret { get; set; } = string.Empty;
		public string SenderEmail { get; set; } = string.Empty;
		public string SenderName { get; set; } = string.Empty;
	}
}
