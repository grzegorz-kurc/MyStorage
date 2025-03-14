﻿using System.ComponentModel.DataAnnotations;

namespace MyStorageAPI.Models.Requests
{
	public class RegisterRequest
	{
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email format.")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Password is required.")]
		[MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
		public string Password { get; set; } = string.Empty;
	}
}