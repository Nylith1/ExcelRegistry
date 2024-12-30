using ExcelRegistry.Shared.Dtos.CosmosDb;

namespace ExcelRegistry.Access.CosmosDb.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<string>> GetUserRoleNamesAsync(string email, CancellationToken cancellationToken);
    Task<IEnumerable<UserDto>> GetUsersAsync(CancellationToken cancellationToken);
    Task InsertUserAsync(UserDto user, CancellationToken cancellationToken);
    Task RemoveUserAsync(string userId, CancellationToken cancellationToken);
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task UpdateUserAsync(UserDto user, CancellationToken cancellationToken);
    Task<UserDto?> GetUserByIdAsync(string id, CancellationToken cancellationToken);
}
