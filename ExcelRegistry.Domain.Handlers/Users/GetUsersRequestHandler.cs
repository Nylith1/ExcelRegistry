using ExcelRegistry.Domain.Handlers.Repositories.User;
using ExcelRegistry.Shared.Dtos.CosmosDb;
using MediatR;

namespace ExcelRegistry.Domain.Handlers.Users;

public class GetUsersRequestHandler(IUserRepository userRepository) : IRequestHandler<GetUsersRequest, IEnumerable<GetUsersResponse>>
{
    public async Task<IEnumerable<GetUsersResponse>> Handle(GetUsersRequest request, CancellationToken cancellationToken)
    {
        var usersData = await userRepository.GetUsersAsync(cancellationToken);
        return MapToResponse(usersData);
    }

    private static IEnumerable<GetUsersResponse> MapToResponse(IEnumerable<UserDto> usersData) => usersData.Select(x => new GetUsersResponse
    {
        Id = x.Id,
        ProfilePhoto = x.Picture,
        Name = x.Name,
        Email = x.Email,
        UserRoleNames = x.Roles.Select(y => y.Name)
    });
}

public record GetUsersRequest : IRequest<IEnumerable<GetUsersResponse>>
{
}

public record GetUsersResponse
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string ProfilePhoto { get; init; }
    public required IEnumerable<string> UserRoleNames { get; init; }
}