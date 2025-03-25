using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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

			// Register AppConfig as IOptions<AppConfig> to allow injection in services (e.g. via IOptions<AppConfig>)
			builder.Services.Configure<AppConfig>(builder.Configuration.GetSection("AppConfig"));

			// Immediately bind AppConfig instance for early access (e.g. for JWT config before builder.Build())
			// Note: Using IOptions<AppConfig> here would not work because the DI container is not yet built
			var appConfig = builder.Configuration.GetSection("AppConfig").Get<AppConfig>();

			if (appConfig is null)
				throw new InvalidOperationException("AppConfig section is missing or invalid.");

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = appConfig.Jwt.Issuer,
					ValidAudience = appConfig.Jwt.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(appConfig.Jwt.SecretKey))
				};
			});

			// Retrieve BaseUrl from configuration
			Console.WriteLine($"Using BaseUrl: {appConfig.BaseUrl}");

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
			builder.Services.AddScoped<IJwtTokenGeneratorService, JwtTokenGeneratorService>();

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