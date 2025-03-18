using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyStorageAPI.Models.Configuration;
using MyStorageAPI.Models.Requests;
using MyStorageAPI.Models.Responses;
using MyStorageAPI.Services.Interfaces;

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
	}
}