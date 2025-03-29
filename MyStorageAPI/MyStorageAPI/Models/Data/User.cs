using Microsoft.AspNetCore.Identity;
using MyStorageAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyStorageAPI.Models.Data
{
	public class User : IdentityUser
	{
		[Required]
		public override string Email { get; set; } = string.Empty;

		public UserRole Role { get; set; } = UserRole.User; // Global role (SuperAdmin, future roles possible)
		public ICollection<WarehouseUser> WarehouseUsers { get; set; } = new List<WarehouseUser>();

		public string DisplayName { get; set; } = string.Empty;
		public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
		public UserSubscription Subscription { get; set; }
		public bool IsVisible { get; set; } = true;

		public ICollection<UserLog> UserLogs { get; set; } = new List<UserLog>();
		public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
	}
}