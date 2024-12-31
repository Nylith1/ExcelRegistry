using ExcelRegistry.Domain.Handlers.Repositories.User;
using ExcelRegistry.Domain.Users;
using ExcelRegistry.Shared;
using ExcelRegistry.Shared.Helpers.Exceptions;
using MediatR;

namespace ExcelRegistry.Domain.Handlers.Users;

public class AddUserRequestHandler(IUserRepository userRepository, IRoleRepository roleRepository) : IRequestHandler<AddUserRequest>
{
    public async Task Handle(AddUserRequest request, CancellationToken cancellationToken)
    {
        await ValidateUserDuplication(request, cancellationToken);
        var user = await CreateUser(request, cancellationToken);
        await userRepository.InsertUserAsync(user.MapToDto(), cancellationToken);
    }

    private async Task<User> CreateUser(AddUserRequest request, CancellationToken cancellationToken)
    {
        var userRoleData = await roleRepository.GetRole(Constants.Roles.User, cancellationToken);
        return User.Create(new()
        {
            Id = Guid.NewGuid().ToString(),
            Email = request.Email,
            Name = request.Name,
            Picture = string.Empty,
            Roles = [new() { Id = userRoleData.Id, Name = userRoleData.Name }]
        });
    }

    private async Task ValidateUserDuplication(AddUserRequest request, CancellationToken cancellationToken)
    {
        var usersData = await userRepository.GetUsersAsync(cancellationToken);
        if (usersData.Any(x => x.Email.ToLower() == request.Email.ToLower()))
        {
            throw new DomainException("User email already exists.");
        }
    }
}

public record AddUserRequest : IRequest
{
    public required string Name { get; init; }
    public required string Email { get; init; }
}