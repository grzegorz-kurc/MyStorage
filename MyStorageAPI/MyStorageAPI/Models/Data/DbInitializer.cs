using MyStorageAPI.Data;
using MyStorageAPI.Models.Enums;

namespace MyStorageAPI.Models.Data
{
	public class DbInitializer
	{
		public static void SeedDatabase(ApplicationDbContext context)
		{
			if (!context.SubscriptionPlans.Any())
			{
				context.SubscriptionPlans.AddRange(new List<SubscriptionPlan>
				{
					new SubscriptionPlan
					{
						Type = SubscriptionType.Free,
						MaxWarehouses = 1,
						MaxCategories = 5,
						MaxProducts = 25,
						MaxUsers = 1
					},
					new SubscriptionPlan
					{
						Type = SubscriptionType.Premium,
						MaxWarehouses = 5,
						MaxCategories = 25,
						MaxProducts = 100,
						MaxUsers = 5
					},
					new SubscriptionPlan
					{
						Type = SubscriptionType.PremiumCustom,
						MaxWarehouses = 20,
						MaxCategories = 100,
						MaxProducts = 1000,
						MaxUsers = 10
					}
				});

				context.SaveChanges();
			}
		}
	}
}