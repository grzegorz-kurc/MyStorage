using MyStorageAPI.Models.Enums;

namespace MyStorageAPI.Models.Data
{
	public class WarehouseUser
	{
		public int WarehouseId { get; set; }
		public Warehouse Warehouse { get; set; }		
		public string UserId { get; set; }
		public User User { get; set; }		
		public UserRole Role { get; set; } = UserRole.User; // User's role within the specific warehouse (Admin or User)
	}
}
