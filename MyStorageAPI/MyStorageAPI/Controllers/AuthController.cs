using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyStorageAPI.Models.Configuration;
using MyStorageAPI.Models.Requests;
using MyStorageAPI.Services.Interfaces;
using System.Security.Claims;

namespace MyStorageAPI.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly ILogger<AuthController> _logger;
		private readonly AppConfig _config;

		public AuthController(IAuthService authService, ILogger<AuthController> logger, IOptions<AppConfig> config)
		{
			_authService = authService;
			_logger = logger;
			_config = config.Value;
		}

		/// <summary>
		/// Registers a new user and sends an email confirmation link.
		/// </summary>
		/// <remarks>
		/// **Sample request:**
		///
		///     POST /api/auth/register
		///     {
		///        "email": "user@example.com",
		///        "password": "SecurePassword123!"
		///     }
		///
		/// After successful registration, an email with a confirmation link is sent.
		/// </remarks>
		/// <param name="request">User registration request model.</param>
		/// <returns>Success message or error details.</returns>
		/// <response code="200">User registered successfully.</response>
		/// <response code="400">
		/// Invalid registration data. Possible reasons:
		/// - Incorrect email format.
		/// - Weak or invalid password.
		/// - Email address is already in use.
		/// </response>
		/// <response code="500">Internal server error.</response>
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
			}

			var baseUrl = string.IsNullOrEmpty(_config.BaseUrl)
					? $"{Request.Scheme}://{Request.Host.Value}"
					: _config.BaseUrl;

			try
			{
				var result = await _authService.RegisterUserAsync(request.Email, request.Password, baseUrl);

				if (!result.Success)
					return BadRequest(new { errors = result.Errors });

				return Ok("User registered successfully. Please check your email for confirmation.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error occurred during user registration.");
				return StatusCode(500, "An error occurred while processing the request.");
			}
		}

		/// <summary>
		/// Confirms the user's email address using the provided confirmation token.
		/// </summary>
		/// <remarks>
		/// **Sample request:**
		///
		///     GET /api/auth/confirm-email?userId=12345&amp;token=abcde12345
		///
		/// The user must click the confirmation link sent to their email.
		/// </remarks>
		/// <param name="userId">The unique identifier of the user.</param>
		/// <param name="token">The email confirmation token.</param>
		/// <returns>A success message or an error response.</returns>
		/// <response code="200">Email confirmed successfully.</response>
		/// <response code="400">
		/// Email confirmation failed. Possible reasons:
		/// - The confirmation token is invalid or expired.
		/// - The user ID does not exist.
		/// </response>
		/// <response code="500">Internal server error.</response>
		[HttpGet("confirm-email")]
		public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
		{
			try
			{
				var result = await _authService.ConfirmEmailAsync(userId, token);
				if (!result)
					return BadRequest("Invalid or expired confirmation token.");

				return Ok("Email confirmed successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error occurred during email confirmation.");
				return StatusCode(500, "An error occurred while processing the request.");
			}
		}

		/// <summary>
		/// Resends the email confirmation link to the user.
		/// </summary>
		/// <remarks>
		/// **Sample request:**
		///
		///     POST /api/auth/resend-confirmation
		///     {
		///        "email": "user@example.com"
		///     }
		///
		/// If the email is not confirmed yet, a new confirmation link is sent.
		/// </remarks>
		/// <param name="request">The request containing the user's email address.</param>
		/// <returns>Success message or error details.</returns>
		/// <response code="200">Confirmation email resent successfully.</response>
		/// <response code="400">
		/// Failed to resend confirmation email. Possible reasons:
		/// - Email address does not exist.
		/// - Email is already confirmed.
		/// </response>
		/// <response code="500">Internal server error.</response>
		[HttpPost("resend-confirmation")]
		public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
			}

			var baseUrl = string.IsNullOrEmpty(_config.BaseUrl)
				? $"{Request.Scheme}://{Request.Host.Value}"
				: _config.BaseUrl;

			try
			{
				var result = await _authService.ResendConfirmationEmailAsync(request.Email, baseUrl);
				if (!result.Success)
					return BadRequest(new { errors = result.Errors });

				return Ok("Confirmation email has been resent. Please check your inbox.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error occurred while resending the confirmation email.");
				return StatusCode(500, "An error occurred while processing the request.");
			}
		}

		/// <summary>
		/// Initiates a password reset request by sending a reset link to the user's email.
		/// Always returns success to prevent user enumeration attacks.
		/// </summary>
		/// <remarks>
		/// **Sample request:**
		///
		///     POST /api/auth/reset-password-request
		///     {
		///        "email": "user@example.com"
		///     }
		///
		/// If the provided email exists in the system, a password reset link will be sent.
		/// This endpoint does not disclose whether the email is registered for security reasons.
		/// </remarks>
		/// <param name="request">Password reset request model containing the user's email.</param>
		/// <returns>A generic success message.</returns>
		/// <response code="200">
		/// If an account with this email exists, a password reset email has been sent.
		/// </response>
		/// <response code="400">
		/// Password reset request failed. Possible reasons:
		/// - The email format is invalid or missing.
		/// </response>
		/// <response code="500">Internal server error.</response>
		[HttpPost("reset-password-request")]
		public async Task<IActionResult> ResetPasswordRequest([FromBody] ForgotPasswordRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
				}

				await _authService.SendPasswordResetEmailAsync(request.Email);

				return Ok("If an account with this email exists, a password reset email has been sent.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error occurred during password reset request.");
				return StatusCode(500, "An error occurred while processing the request.");
			}
		}

		/// <summary>
		/// Resets the password for a user using the reset token.
		/// </summary>
		/// <remarks>
		/// **Sample request:**
		///
		///     POST /api/auth/reset-password
		///     {
		///        "userId": "56309152-2154-4a0f-9e1d-df2dab2f1ef8",
		///        "token": "base64-encoded-token",
		///        "newPassword": "NewSecurePassword123!",
		///        "confirmPassword": "NewSecurePassword123!"
		///     }
		///
		/// The user must provide a valid reset token received via email.
		/// </remarks>
		/// <param name="request">Password reset request model.</param>
		/// <returns>A success message or an error response.</returns>
		/// <response code="200">Password reset successfully.</response>
		/// <response code="400">
		/// Password reset failed. Possible reasons:
		/// - Invalid or expired token.
		/// - Weak or invalid password.
		/// - New password and confirmation do not match.
		/// </response>
		/// <response code="500">Internal server error.</response>
		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
				}

				var result = await _authService.ResetPasswordAsync(request.UserId, request.Token, request.NewPassword);
				if (!result.Success)
					return BadRequest(new { errors = result.Errors });

				return Ok("Password has been successfully reset.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error occurred during password reset.");
				return StatusCode(500, "An error occurred while processing the request.");
			}
		}

		/// <summary>
		/// Authenticates the user and returns a JWT token.
		/// </summary>
		/// <remarks>
		/// **Sample request:**
		///
		///     POST /api/auth/login
		///     {
		///        "email": "user@example.com",
		///        "password": "SecurePassword123!"
		///     }
		///
		/// </remarks>
		/// <param name="request">Login request containing email and password.</param>
		/// <returns>A JWT token if authentication succeeds, or error messages.</returns>
		/// <response code="200">Login successful, token returned.</response>
		/// <response code="400">Login failed. Invalid credentials or request.</response>
		/// <response code="500">Internal server error.</response>
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
				}

				var result = await _authService.LoginAsync(request.Email, request.Password);
				if (!result.Success)
				{
					return BadRequest(new { errors = result.Errors });
				}

				return Ok(result.Response);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error occurred during login.");
				return StatusCode(500, "An error occurred while processing the request.");
			}
		}

		/// <summary>
		/// Refreshes JWT access token using a valid refresh token.
		/// </summary>
		/// <remarks>
		/// **Sample request:**
		///
		///     POST /api/auth/refresh
		///     {
		///         "expiredAccessToken": "eyJhbGciOi...",
		///         "refreshToken": "Qx7bJML5...=="  
		///     }
		/// </remarks>
		/// <param name="request">Refresh token request model.</param>
		/// <returns>New access and refresh token or error message.</returns>
		/// <response code="200">Token refreshed successfully.</response>
		/// <response code="400">Invalid or expired token.</response>
		/// <response code="500">Internal server error.</response>
		[HttpPost("refresh")]
		public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(new
					{
						errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
					});
				}

				var result = await _authService.RefreshTokenAsync(request.ExpiredAccessToken, request.RefreshToken);

				if (!result.Success)
					return BadRequest(new { errors = result.Errors });

				return Ok(result.Response);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error occurred while refreshing token.");
				return StatusCode(500, "An error occurred while processing the request.");
			}
		}

		/// <summary>
		/// Changes the password for a logged-in user.
		/// </summary>
		/// <remarks>
		/// **Sample request:**
		///
		///     POST /api/auth/change-password
		///     {
		///         "currentPassword": "OldPassword123!",
		///         "newPassword": "NewPassword123!",
		///         "confirmPassword": "NewPassword123!"
		///     }
		/// </remarks>
		/// <param name="request">Change password request model.</param>
		/// <returns>Success message or error details.</returns>
		/// <response code="200">Password changed successfully.</response>
		/// <response code="400">Invalid request or password change failed.</response>
		/// <response code="401">Unauthorized.</response>
		/// <response code="500">Internal server error.</response>
		[Authorize]
		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
				}

				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized("User ID not found in token.");
				}

				var result = await _authService.ChangePasswordAsync(userId, request);
				if (!result.Success)
				{
					return BadRequest(new { errors = result.Errors });
				}

				return Ok("Password changed successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error occurred during password change.");
				return StatusCode(500, "An error occurred while processing the request.");
			}
		}
	}
}