using ExcelRegistry.Access.CosmosDb.Repositories.Interfaces;
using MediatR;

namespace ExcelRegistry.Domain.Handlers.Users;

public class RemoveUserRequestHandler(IUserRepository userRepository) : IRequestHandler<RemoveUserRequest>
{
    public async Task Handle(RemoveUserRequest request, CancellationToken cancellationToken)
    {
        await userRepository.RemoveUserAsync(request.UserId, cancellationToken);
    }
}

public record RemoveUserRequest : IRequest
{
    public required string UserId { get; init; }
}