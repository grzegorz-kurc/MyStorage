using Microsoft.EntityFrameworkCore;
using MyStorageAPI.Data;
using MyStorageAPI.Models.Data;
using MyStorageAPI.Services.Interfaces;
using MyStorageAPI.Services;
using Serilog;
using MyStorageAPI.Models.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace MyStorageAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Load configuration
			builder.Services.Configure<AppConfig>(builder.Configuration.GetSection("AppConfig"));

			// Retrieve BaseUrl from configuration
			var baseUrl = builder.Configuration.GetValue<string>("AppConfig:BaseUrl");
			Console.WriteLine($"Using BaseUrl: {baseUrl}");

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
			builder.Services.AddSwaggerGen(options =>
			{
				var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
			});

			// Configure Identity & Token Lifespan
			builder.Services.AddIdentity<User, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
			{
				options.TokenLifespan = TimeSpan.FromMinutes(30);
			});

			// Register services
			builder.Services.AddScoped<IAuthService, AuthService>();
			builder.Services.AddHttpClient<IEmailService, EmailService>();

			var app = builder.Build();

			// Run database seeding (if needed)
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

			// Enable Authentication & Authorization
			app.UseHttpsRedirection();
			app.UseAuthentication();
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