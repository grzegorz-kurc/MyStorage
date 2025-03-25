namespace MyStorageAPI.Models.Responses
{
	public class JwtTokenResult
	{
		public string Token { get; set; } = string.Empty;
		public DateTime Expiration { get; set; }
		public string RefreshToken { get; set; } = string.Empty;
		public DateTime RefreshTokenExpiration { get; set; }
	}
}