using MyStorageAPI.Models.Enums;

namespace MyStorageAPI.Models.Data
{
	public class UserLog
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public User User { get; set; }
		public UserAction Action { get; set; }
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;
	}
}