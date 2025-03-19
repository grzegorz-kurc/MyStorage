﻿using System.ComponentModel.DataAnnotations;

namespace MyStorageAPI.Models.Requests
{
	public class ForgotPasswordRequest
	{
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email format.")]
		public string Email { get; set; } = string.Empty;
	}
}