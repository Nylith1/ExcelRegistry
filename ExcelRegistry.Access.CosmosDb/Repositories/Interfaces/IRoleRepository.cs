using ExcelRegistry.Shared.Dtos.CosmosDb;

namespace ExcelRegistry.Access.CosmosDb.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<RoleDto> GetRole(string roleName, CancellationToken cancellationToken);
    Task InsertRoleAsync(RoleDto role, CancellationToken cancellationToken);
}
