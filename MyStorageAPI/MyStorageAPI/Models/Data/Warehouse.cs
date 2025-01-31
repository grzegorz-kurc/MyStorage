namespace MyStorageAPI.Models.Data
{
	public class Warehouse
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;	
		public string OwnerId { get; set; } = string.Empty; // Warehouse owner (Admin)
		public User Owner { get; set; }	
		public ICollection<Category> Categories { get; set; } = new List<Category>(); // List of product categories within the warehouse
		public ICollection<WarehouseUser> WarehouseUsers { get; set; } = new List<WarehouseUser>(); // List of users who have access to the warehouse
	}
}
