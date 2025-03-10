using MyStorageAPI.Models.Enums;

namespace MyStorageAPI.Models.Data
{
	public class SubscriptionPlan
	{
		public int Id { get; set; }
		public SubscriptionType Type { get; set; }

		public int MaxWarehouses { get; set; }
		public int MaxCategories { get; set; }
		public int MaxProducts { get; set; }
		public int MaxUsers { get; set; }
	}
}