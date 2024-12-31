using ExcelRegistry.Domain.Common;
using ExcelRegistry.Domain.Handlers.Repositories.User;
using ExcelRegistry.Domain.Users;
using ExcelRegistry.Shared;
using MediatR;

namespace ExcelRegistry.Domain.Handlers.Users;

public class RevokeAdminRoleRequestHandler(IUserRepository userRepository) : IRequestHandler<RevokeAdminRoleRequest>
{
    public async Task Handle(RevokeAdminRoleRequest request, CancellationToken cancellationToken)
    {
        var userData = await userRepository.GetUserByIdAsync(request.UserId, cancellationToken) ?? throw new Exception($"User can not be found with id: {request.UserId}.");
        var adminRoleData = userData.Roles.FirstOrDefault(x =>x.Name == Constants.Roles.Admin) ?? throw new Exception($"No admin role found in database.");

        var user = User.Create(userData);
        user.RemoveRole(GuidId.Create(adminRoleData.Id));

        await userRepository.UpdateUserAsync(user.MapToDto(), cancellationToken);
    }
}

public record RevokeAdminRoleRequest : IRequest
{
    public required string UserId { get; init; }
}