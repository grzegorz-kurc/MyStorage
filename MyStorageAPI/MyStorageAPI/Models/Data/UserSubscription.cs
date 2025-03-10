using MyStorageAPI.Models.Enums;

namespace MyStorageAPI.Models.Data
{
	public class UserSubscription
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public User User { get; set; }

		public int SubscriptionPlanId { get; set; }
		public SubscriptionPlan Plan { get; set; }

		public DateTime StartDate { get; set; } = DateTime.UtcNow;
		// Expiration date (null means no expiration)
		public DateTime? ExpirationDate { get; set; } = null;
	}
}