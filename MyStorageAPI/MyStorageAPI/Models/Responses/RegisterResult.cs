namespace MyStorageAPI.Models.Responses
{
	public class RegisterResult
	{
		public bool Success { get; set; }
		public List<string> Errors { get; set; } = new List<string>();
	}
}
