namespace MyStorageAPI.Models.Responses
{
	public class LoginResult
	{
		public bool Success { get; set; }
		public List<string> Errors { get; set; } = new();
		public LoginResponse? Response { get; set; }
	}
}