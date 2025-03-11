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

			modelBuilder.Entity<WarehouseUser>()
				.HasKey(wu => wu.Id);

			// Relation 1-N: Warehouse → WarehouseUser
			modelBuilder.Entity<WarehouseUser>()
				.HasOne(wu => wu.Warehouse)
				.WithMany(w => w.WarehouseUsers)
				.HasForeignKey(wu => wu.WarehouseId)
				.OnDelete(DeleteBehavior.NoAction);

			// Relation 1-N: User → WarehouseUser
			modelBuilder.Entity<WarehouseUser>()
				.HasOne(wu => wu.User)
				.WithMany(u => u.WarehouseUsers)
				.HasForeignKey(wu => wu.UserId)
				.OnDelete(DeleteBehavior.NoAction);

			// Relation 1-N: SubscriptionPlan → UserSubscription
			modelBuilder.Entity<UserSubscription>()
				.HasOne(us => us.Plan)
				.WithMany()
				.HasForeignKey(us => us.SubscriptionPlanId);

			// Relation 1-1: User → UserSubscription
			modelBuilder.Entity<User>()
				.HasOne(u => u.Subscription)
				.WithOne(us => us.User)
				.HasForeignKey<UserSubscription>(us => us.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			// Cascade delete for UserLogs (deleting a user removes their logs)
			modelBuilder.Entity<UserLog>()
				.HasOne(ul => ul.User)
				.WithMany(u => u.UserLogs)
				.HasForeignKey(ul => ul.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			// NoAction for User → Warehouse (do not delete warehouses when a user is removed)
			modelBuilder.Entity<Warehouse>()
				.HasOne(w => w.Owner)
				.WithMany()
				.HasForeignKey(w => w.OwnerId)
				.OnDelete(DeleteBehavior.NoAction);

			// Cascade delete for ProductLogs (deleting a product removes its logs)
			modelBuilder.Entity<Product>()
				.HasMany(p => p.ProductLogs)
				.WithOne(pl => pl.Product)
				.HasForeignKey(pl => pl.ProductId)
				.OnDelete(DeleteBehavior.Cascade);

			// NoAction for Category → Product (to prevent automatic deletion of products)
			modelBuilder.Entity<Category>()
				.HasMany(c => c.Products)
				.WithOne(p => p.Category)
				.HasForeignKey(p => p.CategoryId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}