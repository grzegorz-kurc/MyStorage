using Microsoft.IdentityModel.Tokens;
using MyStorageAPI.Models.Configuration;
using MyStorageAPI.Models.Data;
using MyStorageAPI.Models.Responses;
using MyStorageAPI.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace MyStorageAPI.Services
{
	public class JwtTokenGeneratorService : IJwtTokenGeneratorService
	{
		private readonly AppConfig _config;

		public JwtTokenGeneratorService(IOptions<AppConfig> config)
		{
			_config = config.Value;
		}

		/// <summary>
		/// Generates a new pair of tokens: access token (JWT) and refresh token.
		/// </summary>
		public JwtTokenResult GenerateTokens(User user)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Email, user.Email ?? ""),
				new Claim(ClaimTypes.Name, user.UserName ?? "")
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Jwt.SecretKey));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expiration = DateTime.UtcNow.AddMinutes(_config.Jwt.TokenLifetimeMinutes);

			var tokenDescriptor = new JwtSecurityToken(
				issuer: _config.Jwt.Issuer,
				audience: _config.Jwt.Audience,
				claims: claims,
				expires: expiration,
				signingCredentials: credentials
			);

			var jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

			// Generate a secure refresh token
			var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
			var refreshTokenExpiration = DateTime.UtcNow.AddDays(_config.Jwt.RefreshTokenLifetimeDays);

			return new JwtTokenResult
			{
				Token = jwt,
				Expiration = expiration,
				RefreshToken = refreshToken,
				RefreshTokenExpiration = refreshTokenExpiration
			};
		}
	}
}