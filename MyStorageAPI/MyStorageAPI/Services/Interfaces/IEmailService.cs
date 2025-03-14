using MyStorageAPI.Models.Responses;

namespace MyStorageAPI.Services.Interfaces
{
    public interface IEmailService
    {
		Task<SendEmailResult> SendEmailAsync(string to, string subject, string body);
	}
}