namespace MyStorageAPI.Models.Responses
{
	public class SendEmailResult
	{
		public bool Success { get; set; }
		public List<string> Errors { get; set; } = new List<string>();
	}
}