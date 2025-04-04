namespace MyStorageAPI.Models.Requests
{
	public class RefreshTokenRequest
	{
		public string ExpiredAccessToken { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
	}
}