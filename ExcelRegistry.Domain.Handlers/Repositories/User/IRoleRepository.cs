using ExcelRegistry.Shared.Dtos.CosmosDb;

namespace ExcelRegistry.Domain.Handlers.Repositories.User;

public interface IRoleRepository
{
    Task<RoleDto> GetRole(string roleName, CancellationToken cancellationToken);
    Task InsertRoleAsync(RoleDto role, CancellationToken cancellationToken);
}

