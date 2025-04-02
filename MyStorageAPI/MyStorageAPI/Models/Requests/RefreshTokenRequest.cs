namespace MyStorageAPI.Models.Requests
{
	public class RefreshTokenRequest
	{
		public string Token { get; set; } = string.Empty; // expired access token
		public string RefreshToken { get; set; } = string.Empty;
	}
}