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
			var result = await _authService.RegisterUserAsync(request.Email, request.Password);

			if (!result.Success)
				return BadRequest(new { errors = result.Errors });

			return Ok("User registered successfully.");
		}
	}
}