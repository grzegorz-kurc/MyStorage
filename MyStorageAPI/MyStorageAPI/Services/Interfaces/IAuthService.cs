using MyStorageAPI.Models.Responses;

namespace MyStorageAPI.Services.Interfaces
{
	public interface IAuthService
	{
		Task<RegisterResult> RegisterUserAsync(string email, string password, string clientBaseUrl);
		Task<bool> ConfirmEmailAsync(string userId, string token);
	}
}