using ExcelRegistry.Access.CosmosDb.Repositories.Interfaces;
using ExcelRegistry.Shared.Dtos.CosmosDb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ExcelRegistry.Functions.Functions;

public class SeedUsersDbWithInitialDataFunction(IUserRepository userRepository, IRoleRepository roleRepository, ILogger<SeedUsersDbWithInitialDataFunction> logger)
{
    [Function(nameof(SeedUsersDbWithInitialDataFunction))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] CancellationToken cancellationToken)
    {
        List<UserDto> users = [
           new()
            {
                Id = "304d1bf5-289b-4a7c-899c-6dc66147d06c",
                Email = "dainius.tenys@gmail.com",
                Name = "Dainius Tenys",
                Picture = "https://lh3.googleusercontent.com/a/ACg8ocKt6XwYjObfT3xLnabWP-U-ODt0kyfQAwwxtBIDV716Owhbtg=s96-c",
                Roles = [new() { Id = "968394d5-7bc0-4d95-bb3e-250a987746ef", Name = Shared.Constants.Roles.User.ToString() },
                         new() { Id = "7e60b13b-976b-4ab0-aa79-1044bed43534", Name = Shared.Constants.Roles.Admin.ToString() }]
           },
           new()
           {
               Id = "5d09b9ed-1390-4dd3-9659-26f5ee8d3a4c",
               Email = "nannislasas@gmail.com",
               Name = "Nylith Taip",
               Picture = "https://lh3.googleusercontent.com/a/ACg8ocKPZpkqrHWSUxKHBXzgZEBiI3NM1s2g6Qq3IPNqU6w8OX3Abw=s96-c",
               Roles = [new() { Id = "968394d5-7bc0-4d95-bb3e-250a987746ef", Name = Shared.Constants.Roles.User.ToString() }]
           }
        ];

        List<RoleDto> roles = [new() { Id = "968394d5-7bc0-4d95-bb3e-250a987746ef", Name = Shared.Constants.Roles.User.ToString() },
                            new() { Id = "7e60b13b-976b-4ab0-aa79-1044bed43534", Name = Shared.Constants.Roles.Admin.ToString() }];

        try
        {
            foreach (var user in users)
            {
                await userRepository.InsertUserAsync(user, cancellationToken);
            }
            foreach (var role in roles)
            {
                await roleRepository.InsertRoleAsync(role, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(message: $"Error while running {nameof(SeedUsersDbWithInitialDataFunction)}. Error: {ex.Message}");
            return new ObjectResult(new { message = "An error occurred while processing your request.", error = ex.Message })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        return new OkObjectResult($"{nameof(SeedUsersDbWithInitialDataFunction)} ran successfuly");
    }
}
