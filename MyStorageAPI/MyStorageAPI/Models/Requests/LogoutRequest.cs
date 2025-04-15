namespace MyStorageAPI.Models.Requests
{
	public class LogoutRequest
	{
		public string RefreshToken { get; set; } = string.Empty;
	}
}