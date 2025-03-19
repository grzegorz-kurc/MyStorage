using System.ComponentModel.DataAnnotations;

namespace MyStorageAPI.Models.Requests
{
	public class ResetPasswordRequest
	{
		[Required(ErrorMessage = "User ID is required.")]
		public string UserId { get; set; } = string.Empty;

		[Required(ErrorMessage = "Token is required.")]
		public string Token { get; set; } = string.Empty;

		[Required(ErrorMessage = "New password is required.")]
		[MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
		public string NewPassword { get; set; } = string.Empty;

		[Required(ErrorMessage = "Password confirmation is required.")]
		[Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}