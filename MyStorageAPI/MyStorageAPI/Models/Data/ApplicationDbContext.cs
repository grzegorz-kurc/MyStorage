using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyStorageAPI.Models.Data;

namespace MyStorageAPI.Data
{
	public class ApplicationDbContext : IdentityDbContext<User>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options) { }

		public DbSet<Warehouse> Warehouses { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<ProductLog> ProductLogs { get; set; }
		public DbSet<UserLog> UserLogs { get; set; }
		public DbSet<WarehouseUser> WarehouseUsers { get; set; }
		public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
		public DbSet<UserSubscription> UserSubscriptions { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Composite key for WarehouseUser
			modelBuilder.Entity<WarehouseUser>()
				.HasKey(wu => new { wu.WarehouseId, wu.UserId });

			// Relation 1-N: Warehouse → WarehouseUser
			modelBuilder.Entity<WarehouseUser>()
				.HasOne(wu => wu.Warehouse)
				.WithMany(w => w.WarehouseUsers)
				.HasForeignKey(wu => wu.WarehouseId);

			// Relation 1-N: User → WarehouseUser
			modelBuilder.Entity<WarehouseUser>()
				.HasOne(wu => wu.User)
				.WithMany(u => u.WarehouseUsers)
				.HasForeignKey(wu => wu.UserId);

			// Relation 1-N: SubscriptionPlan → UserSubscription
			modelBuilder.Entity<UserSubscription>()
				.HasOne(us => us.Plan)
				.WithMany()
				.HasForeignKey(us => us.SubscriptionPlanId);

			// Relation 1-1: User → UserSubscription
			modelBuilder.Entity<User>()
				.HasOne(u => u.Subscription)
				.WithOne(us => us.User)
				.HasForeignKey<UserSubscription>(us => us.UserId);

			// Automatically delete logs when a product is deleted
			modelBuilder.Entity<Product>()
				.HasMany(p => p.ProductLogs)
				.WithOne(pl => pl.Product)
				.HasForeignKey(pl => pl.ProductId)
				.OnDelete(DeleteBehavior.Cascade);

			// Automatically delete products when a category is deleted
			modelBuilder.Entity<Category>()
				.HasMany(c => c.Products)
				.WithOne(p => p.Category)
				.HasForeignKey(p => p.CategoryId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}