using ExcelRegistry.Access.CosmosDb.Repositories.Interfaces;
using ExcelRegistry.Domain.Users;
using ExcelRegistry.Shared;
using MediatR;

namespace ExcelRegistry.Domain.Handlers.Users;

public class GrantAdminRoleRequestHandler(IUserRepository userRepository, IRoleRepository roleRepository) : IRequestHandler<GrantAdminRoleRequest>
{
    public async Task Handle(GrantAdminRoleRequest request, CancellationToken cancellationToken)
    {
        var userData = await userRepository.GetUserByIdAsync(request.UserId, cancellationToken) ?? throw new Exception($"User can not be found with id: {request.UserId}");
        var adminRoleData = await roleRepository.GetRole(Constants.Roles.Admin, cancellationToken);

        var user = User.Create(userData);
        user.AddRole(adminRoleData);

        await userRepository.UpdateUserAsync(user.MapToDto(), cancellationToken);
    }
}

public record GrantAdminRoleRequest : IRequest
{
    public required string UserId { get; init; }
}