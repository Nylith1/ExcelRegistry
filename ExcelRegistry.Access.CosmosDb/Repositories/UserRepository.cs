using ExcelRegistry.Access.CosmosDb.Databases.Interfaces;
using ExcelRegistry.Access.CosmosDb.Models.DataModels;
using ExcelRegistry.Domain.Handlers.Repositories.User;
using ExcelRegistry.Shared;
using ExcelRegistry.Shared.Dtos.CosmosDb;

namespace ExcelRegistry.Access.CosmosDb.Repositories;

public class UserRepository(IExcelRegistryUserDb userDb) : IUserRepository
{
    public async Task<IEnumerable<string>> GetUserRoleNamesAsync(string email, CancellationToken cancellationToken)
    {
        var usersData = await userDb.GetRecordsAsync<UserData>(Constants.ExcelRegistryUsersDbTables.Users, cancellationToken);
        var user = usersData.FirstOrDefault(x => x.Email == email);
        return user is null ? throw new Exception($"User not found by email {email}") : (IEnumerable<string>)user.Roles.Select(x => x.Name).ToList();
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var usersData = await userDb.GetRecordsAsync<UserData>(Constants.ExcelRegistryUsersDbTables.Users, cancellationToken);
        return usersData.Select(x => x.MapToDto());
    }

    public async Task InsertUserAsync(UserDto user, CancellationToken cancellationToken)
    {
        UserData userData = MapToUserData(user);
        await userDb.InsertRecordsAsync(Constants.ExcelRegistryUsersDbTables.Users, userData, cancellationToken);
    }

    public async Task RemoveUserAsync(string userId, CancellationToken cancellationToken)
    {
        await userDb.RemoveRecordAsync<UserData>(Constants.ExcelRegistryUsersDbTables.Users, Guid.Parse(userId), cancellationToken);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var users = await userDb.GetRecordsAsync<UserData>(Constants.ExcelRegistryUsersDbTables.Users, cancellationToken);
        var user = users.FirstOrDefault(x => x.Email == email);
        return user?.MapToDto();
    }

    public async Task UpdateUserAsync(UserDto user, CancellationToken cancellationToken)
    {
        var userData = MapToUserData(user);
        await userDb.UpdateRecordAsync(Constants.ExcelRegistryUsersDbTables.Users, userData, Guid.Parse(userData.Id), cancellationToken);
    }

    public async Task<UserDto?> GetUserByIdAsync(string id, CancellationToken cancellationToken)
    {
        var users = await userDb.GetRecordsAsync<UserData>(Constants.ExcelRegistryUsersDbTables.Users, cancellationToken);
        var user = users.FirstOrDefault(x => x.Id == id);
        return user?.MapToDto();
    }

    private static UserData MapToUserData(UserDto user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Name = user.Name,
        Picture = user.Picture,
        Roles = MapToRolesData(user.Roles)
    };

    private static IEnumerable<RoleData> MapToRolesData(IEnumerable<RoleDto> roles) => roles.Select(x => new RoleData
    {
        Id = x.Id,
        Name = x.Name
    });
}
