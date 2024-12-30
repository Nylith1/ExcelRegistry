using ExcelRegistry.Access.CosmosDb.Databases.Interfaces;
using ExcelRegistry.Access.CosmosDb.Models.DataModels;
using ExcelRegistry.Access.CosmosDb.Repositories.Interfaces;
using ExcelRegistry.Shared;
using ExcelRegistry.Shared.Dtos.CosmosDb;

namespace ExcelRegistry.Access.CosmosDb.Repositories;

public class RoleRepository(IExcelRegistryUserDb userDb) : IRoleRepository
{
    public async Task InsertRoleAsync(RoleDto role, CancellationToken cancellationToken)
    {
        var roleData = new RoleData 
        { 
            Id = role.Id,
            Name = role.Name,
        };

        await userDb.InsertRecordsAsync(Constants.ExcelRegistryUsersDbTables.Roles, roleData, cancellationToken);
    }

    public async Task<RoleDto> GetRole(string roleName, CancellationToken cancellationToken)
    {
        var roles = await userDb.GetRecordsAsync<RoleData>(Constants.ExcelRegistryUsersDbTables.Roles, cancellationToken);
        var role = roles.FirstOrDefault(x => x.Name == roleName);
        return role is null ? throw new Exception($"Role can not be found with the name of {roleName}") : new RoleDto { Id = role.Id, Name = role.Name};
    }
}
