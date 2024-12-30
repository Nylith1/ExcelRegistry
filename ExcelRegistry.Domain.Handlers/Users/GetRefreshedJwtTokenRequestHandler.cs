using ExcelRegistry.Domain.Handlers.Services.Interfaces;
using MediatR;

namespace ExcelRegistry.Domain.Handlers.Users;

public class GetRefreshedJwtTokenRequestHandler(ICurrentUserService currentUserService, IJwtTokenService jwtTokenService) : IRequestHandler<GetRefreshedJwtTokenRequest, GetRefreshedJwtTokenResponse>
{
    public async Task<GetRefreshedJwtTokenResponse> Handle(GetRefreshedJwtTokenRequest request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(new GetRefreshedJwtTokenResponse { JwtToken = jwtTokenService.Generate(currentUserService.User.Claims) });
    }
}

public record GetRefreshedJwtTokenRequest : IRequest<GetRefreshedJwtTokenResponse>
{
}

public record GetRefreshedJwtTokenResponse
{
    public required string JwtToken { get; init; }
}