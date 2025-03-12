
using Microsoft.EntityFrameworkCore;
using MyStorageAPI.Data;
using MyStorageAPI.Models.Data;
using Serilog;

namespace MyStorageAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Load Serilog configuration from appsettings.json
			builder.Host.UseSerilog((context, services, configuration) =>
				configuration.ReadFrom.Configuration(context.Configuration));

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

			// Manually run database seeding without migrations
			using (var scope = app.Services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				DbInitializer.SeedDatabase(dbContext);
			}

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

			try
			{
				Log.Information("Starting MyStorage API...");
				app.Run();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Application terminated unexpectedly.");
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}
	}
}
