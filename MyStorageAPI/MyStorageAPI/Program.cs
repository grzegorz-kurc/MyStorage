
using Microsoft.EntityFrameworkCore;
using MyStorageAPI.Data;

namespace MyStorageAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

			// Register db in DI container (Entity Framework Core + SQL Server)
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));

			// Add Controllers
			builder.Services.AddControllers();

			// Add Swagger
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// Enable Swagger in Development Mode
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			// Use HTTPS
			app.UseHttpsRedirection();

			// app.UseAuthentication(); TODO: Add Identity
			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}
