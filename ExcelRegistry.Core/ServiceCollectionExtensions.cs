using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using ExcelRegistry.Access.CosmosDb.Databases;
using ExcelRegistry.Access.CosmosDb.Databases.Interfaces;
using ExcelRegistry.Access.CosmosDb.Repositories;
using ExcelRegistry.Access.GoogleSheets.Repositories;
using ExcelRegistry.Access.GoogleSheets.Services;
using ExcelRegistry.Access.GoogleSheets.Services.Interfaces;
using ExcelRegistry.Domain.Handlers.Repositories.Regent;
using ExcelRegistry.Domain.Handlers.Repositories.User;
using ExcelRegistry.Domain.Handlers.Services;
using ExcelRegistry.Domain.Handlers.Services.Interfaces;
using ExcelRegistry.Shared.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ExcelRegistry.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureExcelRegistryServices(this IServiceCollection services)
        {
            ConfigureAccessGoogleSheetsServices(services);
            ConfigureJwtAuthentication(services);
            ConfigureDomainHandlers(services);
            ConfigureAccessCosmosDb(services);
            return services;
        }

        public static void ConfigureOptions(this WebApplicationBuilder builder)
        {
            var keyVaultUri = builder.Configuration.GetSection("KeyVault:KeyVaultURL");
            var clientId = builder.Configuration.GetSection("KeyVault:ClientId");
            var clientSecret = builder.Configuration.GetSection("KeyVault:ClientSecret");
            var directoryId = builder.Configuration.GetSection("KeyVault:DirectoryId");

            var credential = new ClientSecretCredential(directoryId.Value!.ToString(), clientId.Value!.ToString(), clientSecret.Value!.ToString());
            var secretClient = new SecretClient(new Uri(keyVaultUri.Value!.ToString()), credential);
            builder.Services.Configure<GoogleOAuthOptions>(options =>
            {
                options.GoogleOAuthClientId = secretClient.GetSecret("Google-OAuth-Client-Id").Value.Value;
            });

            builder.Services.Configure<GoogleSheetsOptions>(options =>
            {
                options.JsonCredentials = secretClient.GetSecret("Google-Sheets-Json-Credentials").Value.Value;
                options.SpreadSheetId = secretClient.GetSecret("Spread-Sheet-Id").Value.Value;
            });

            builder.Services.Configure<JwtOptions>(options =>
            {
                options.Audience = builder.Configuration.GetSection("Jwt:Audience").Value!;
                options.Issuer = builder.Configuration.GetSection("Jwt:Issuer").Value!;
                options.Secret = secretClient.GetSecret("Jwt-Secret").Value.Value;
            });

            builder.Services.Configure<ExcelRegistryUserDbOptions>(options =>
            {
                options.ConnectionString = secretClient.GetSecret("ExcelRegistryUserDb-MongoDb-ConnectionString").Value.Value;
            });
        }

        private static void ConfigureAccessGoogleSheetsServices(IServiceCollection services)
        {
            services.AddScoped<IGoogleSheetsService, GoogleSheetsService>();
            services.AddScoped<ISheetsServiceFactory, SheetsServiceFactory>();
            services.AddScoped<IRegentRepository, RegentRepository>();
        }

        public static IServiceCollection ConfigureJwtAuthentication(IServiceCollection services)
        {
            var jwtOptions = services.BuildServiceProvider().GetRequiredService<IOptions<JwtOptions>>().Value;
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
                };
            });

            return services;
        }

        private static void ConfigureDomainHandlers(IServiceCollection services)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
        }

        private static void ConfigureAccessCosmosDb(IServiceCollection services)
        {
            services.AddScoped<IExcelRegistryUserDb, ExcelRegistryUserDb>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
        }
    }
}
