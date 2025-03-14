using Microsoft.AspNetCore.Mvc;
using MyStorageAPI.Models.Requests;
using MyStorageAPI.Services.Interfaces;

namespace MyStorageAPI.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request)
		{
			var clientBaseUrl = $"{Request.Scheme}://{Request.Host.Value}";

			var result = await _authService.RegisterUserAsync(request.Email, request.Password, clientBaseUrl);

			if (!result.Success)
				return BadRequest(new { errors = result.Errors });

			return Ok("User registered successfully. Please check your email for confirmation.");
		}

		[HttpGet("confirm-email")]
		public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
		{
			var result = await _authService.ConfirmEmailAsync(userId, token);
			if (!result)
				return BadRequest("Invalid or expired confirmation token.");

			return Ok("Email confirmed successfully.");
		}
	}
}