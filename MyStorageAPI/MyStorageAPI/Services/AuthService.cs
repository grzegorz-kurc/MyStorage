using Microsoft.AspNetCore.Identity;
using MyStorageAPI.Models.Data;
using MyStorageAPI.Models.Responses;
using MyStorageAPI.Services.Interfaces;

public class AuthService : IAuthService
{
	private readonly UserManager<User> _userManager;
	private readonly IEmailService _emailService;

	public AuthService(UserManager<User> userManager, IEmailService emailService)
	{
		_userManager = userManager;
		_emailService = emailService;
	}

	public async Task<RegisterResult> RegisterUserAsync(string email, string password)
	{
		var user = new User { UserName = email, Email = email };

		var result = await _userManager.CreateAsync(user, password);
		if (!result.Succeeded)
		{
			return new RegisterResult { Success = false, Errors = result.Errors.Select(e => e.Description).ToList() };
		}

		try
		{
			var emailSent = await _emailService.SendEmailAsync(user.Email, "Welcome to MyStorage!", $"Hello {user.Email}, your account has been successfully created!");

			if (!emailSent)
			{
				await _userManager.DeleteAsync(user);
				return new RegisterResult { Success = false, Errors = new List<string> { "Failed to send verification email. Please try again later." } };
			}
		}
		catch (Exception ex)
		{
			await _userManager.DeleteAsync(user);
			return new RegisterResult { Success = false, Errors = new List<string> { "Failed to send verification email. Please try again later." } };
		}

		return new RegisterResult { Success = true };
	}
}