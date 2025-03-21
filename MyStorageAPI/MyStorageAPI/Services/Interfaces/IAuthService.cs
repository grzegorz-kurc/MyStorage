using MyStorageAPI.Models.Responses;

namespace MyStorageAPI.Services.Interfaces
{
	public interface IAuthService
	{
		Task<RegisterResult> RegisterUserAsync(string email, string password, string clientBaseUrl);
		Task<bool> ConfirmEmailAsync(string userId, string token);
		Task<RegisterResult> ResendConfirmationEmailAsync(string email, string clientBaseUrl);
		Task<RegisterResult> SendPasswordResetEmailAsync(string email);
		Task<RegisterResult> ResetPasswordAsync(string userId, string token, string newPassword);
	}
}