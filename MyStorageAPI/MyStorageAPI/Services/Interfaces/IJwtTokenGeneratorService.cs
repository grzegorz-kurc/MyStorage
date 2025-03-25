using MyStorageAPI.Models.Data;
using MyStorageAPI.Models.Responses;

namespace MyStorageAPI.Services.Interfaces
{
	public interface IJwtTokenGeneratorService
	{
		JwtTokenResult GenerateTokens(User user);
	}
}