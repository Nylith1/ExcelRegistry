using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using ExcelRegistry.Access.CosmosDb.Databases;
using ExcelRegistry.Access.CosmosDb.Databases.Interfaces;
using ExcelRegistry.Access.CosmosDb.Repositories;
using ExcelRegistry.Access.CosmosDb.Repositories.Interfaces;
using ExcelRegistry.Shared.Options;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureAppConfiguration((context, config) =>
            {
                var settings = context.HostingEnvironment.IsDevelopment()
                    ? config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    : config.AddEnvironmentVariables();
            })
            .Build();

var configuration = host.Services.GetService<IConfiguration>() ?? throw new Exception("Getting configuration failed.");

var keyVaultUri = configuration["KeyVaultURL"];
var clientId = configuration["ClientId"];
var clientSecret = configuration["ClientSecret"];
var directoryId = configuration["DirectoryId"];

var credential = new ClientSecretCredential(directoryId!.ToString(), clientId!.ToString(), clientSecret!.ToString());
var secretClient = new SecretClient(new Uri(keyVaultUri!.ToString()), credential);
builder.Services.Configure<ExcelRegistryUserDbOptions>(options =>
{
    options.ConnectionString = secretClient.GetSecret("ExcelRegistryUserDb-MongoDb-ConnectionString").Value.Value;
});

builder.Services.AddScoped<IExcelRegistryUserDb, ExcelRegistryUserDb>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

builder.Build().Run();
