using ExcelRegistry.Access.CosmosDb;
using ExcelRegistry.Shared;
using ExcelRegistry.Shared.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Authentication;

namespace ExcelRegistry.Functions.Functions;

public class DropUsersDbTablesFunction(ILogger<DropUsersDbTablesFunction> logger, IOptions<ExcelRegistryUserDbOptions> excelRegistryUserDbOptions)
{
    [Function(nameof(DropUsersDbTablesFunction))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest request)
    {
        try
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(new(excelRegistryUserDbOptions.Value.ConnectionString));
            settings.SslSettings = new() { EnabledSslProtocols = SslProtocols.Tls12 };
            var client = new MongoClient(settings);
            var userDb = client.GetDatabase(Constants.Databases.ExcelRegistryUsersDb);

            var tableNames = userDb.ListCollectionNames().ToList();
            foreach (var name in tableNames)
            {
                await userDb.DropCollectionAsync(name);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(message: $"Error while running {nameof(DropUsersDbTablesFunction)}. Error: {ex.Message}");
            return new ObjectResult(new { message = "An error occurred while processing your request.", error = ex.Message })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        return new OkObjectResult($"{nameof(DropUsersDbTablesFunction)} ran successfuly");
    }
}

