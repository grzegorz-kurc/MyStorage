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
		}
	}
}