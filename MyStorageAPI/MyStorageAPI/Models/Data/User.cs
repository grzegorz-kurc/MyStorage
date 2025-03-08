﻿using Microsoft.AspNetCore.Identity;
using MyStorageAPI.Models.Enums;

namespace MyStorageAPI.Models.Data
{
	public class User : IdentityUser
	{		
		public UserRole Role { get; set; } = UserRole.User; // Global role (SuperAdmin, future roles possible)
		public ICollection<WarehouseUser> WarehouseUsers { get; set; } = new List<WarehouseUser>();
	}
}
