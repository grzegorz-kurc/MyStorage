using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using MyStorageAPI.Models.Data;
using MyStorageAPI.Models.Responses;
using MyStorageAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using MyStorageAPI.Models.Configuration;

public class AuthService : IAuthService
{
	private readonly UserManager<User> _userManager;
	private readonly IEmailService _emailService;
	private readonly ILogger<AuthService> _logger;
	private readonly AppConfig _config; 

	public AuthService(UserManager<User> userManager, IEmailService emailService, ILogger<AuthService> logger, IOptions<AppConfig> config)
	{
		_userManager = userManager;
		_emailService = emailService;
		_logger = logger;
		_config = config.Value;
	}

	/// <summary>
	/// Registers a new user and sends an email confirmation link.
	/// </summary>
	public async Task<RegisterResult> RegisterUserAsync(string email, string password, string clientBaseUrl)
	{
		var user = new User { UserName = email, Email = email, EmailConfirmed = false };

		var result = await _userManager.CreateAsync(user, password);
		if (!result.Succeeded)
		{
			return new RegisterResult { Success = false, Errors = result.Errors.Select(e => e.Description).ToList() };
		}

		// Generate a token for account activation
		var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

		// Create an activation link for the user
		var confirmationLink = $"{clientBaseUrl}/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";

		// Send the email
		var emailBody = $"Hello {user.Email},<br/><br/>Please confirm your email by clicking the link below:<br/><a href='{confirmationLink}'>Confirm Email</a>";

		var emailResult = await _emailService.SendEmailAsync(user.Email, "Confirm your MyStorage account", emailBody);

		if (!emailResult.Success)
		{
			_logger.LogError($"Failed to send confirmation email to {user.Email}. User must request a new email manually.");
			return new RegisterResult { Success = false, Errors = new List<string> { "Failed to send confirmation email. Please try again later." } };
		}

		return new RegisterResult { Success = true };
	}

	/// <summary>
	/// Confirms the user's email address using the provided confirmation token.
	/// </summary>
	public async Task<bool> ConfirmEmailAsync(string userId, string token)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null) return false;

		var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
		var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

		return result.Succeeded;
	}

	/// <summary>
	/// Generates a new email confirmation token and resends the confirmation email.
	/// </summary>
	public async Task<RegisterResult> ResendConfirmationEmailAsync(string email, string clientBaseUrl)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user == null || user.EmailConfirmed)
		{
			return new RegisterResult { Success = false, Errors = new List<string> { "Invalid request or email is already confirmed." } };
		}

		// Generate a new confirmation token
		var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
		var confirmationLink = $"{clientBaseUrl}/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";

		// Send the email
		var emailBody = $"Hello {user.Email},<br/><br/>Please confirm your email by clicking the link below:<br/><a href='{confirmationLink}'>Confirm Email</a>";
		var emailResult = await _emailService.SendEmailAsync(user.Email, "Confirm your MyStorage account", emailBody);

		if (!emailResult.Success)
		{
			_logger.LogError($"Failed to resend confirmation email to {user.Email}");
			return new RegisterResult { Success = false, Errors = new List<string> { "Failed to resend confirmation email. Please try again later." } };
		}

		return new RegisterResult { Success = true };
	}
}