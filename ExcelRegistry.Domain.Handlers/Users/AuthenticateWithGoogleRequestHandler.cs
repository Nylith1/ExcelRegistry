using ExcelRegistry.Domain.Handlers.Repositories.User;
using ExcelRegistry.Domain.Handlers.Services.Interfaces;
using ExcelRegistry.Domain.Users.Entities;
using ExcelRegistry.Shared.Dtos.CosmosDb;
using ExcelRegistry.Shared.Helpers.Exceptions;
using ExcelRegistry.Shared.Options;
using Google.Apis.Auth;
using MediatR;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ExcelRegistry.Domain.Handlers.Users;

public class AuthenticateWithGoogleRequestHandler(IOptions<GoogleOAuthOptions> googleOAuthOptions, IJwtTokenService jwtTokenService, IUserRepository userRepository) : IRequestHandler<AuthenticateWithGoogleRequest, UserResponse>
{
    private readonly GoogleOAuthOptions googleOAuthOptions = googleOAuthOptions.Value;

    public async Task<UserResponse> Handle(AuthenticateWithGoogleRequest request, CancellationToken cancellationToken)
    {
        var payload = await VerifyGoogleTokenAsync(request.Token);
        var user = await userRepository.GetUserByEmailAsync(payload.Email, cancellationToken) ?? throw new NotAuthorizedException("User is not registered. Contact administrator to register the user.");
        await UpdateUserIfNeeded(userRepository, payload, user, cancellationToken);

        var jwtToken = jwtTokenService.Generate(await CreateUserClaimsFromPayload(payload, cancellationToken));
        var userRoleNames = await userRepository.GetUserRoleNamesAsync(payload.Email, cancellationToken);
        return MapToUserResponse(payload, jwtToken, userRoleNames, user);
    }

    private async Task<List<Claim>> CreateUserClaimsFromPayload(GoogleJsonWebSignature.Payload userPayload, CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userPayload.Name),
            new(ClaimTypes.Email, userPayload.Email)
        };

        var userRoleNames = await userRepository.GetUserRoleNamesAsync(userPayload.Email, cancellationToken);
        claims.AddRange(userRoleNames.Select(x => new Claim(ClaimTypes.Role, x)));
        return claims;
    }

    private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string token)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [googleOAuthOptions.GoogleOAuthClientId]
            };

            return await GoogleJsonWebSignature.ValidateAsync(token, settings);
        }
        catch
        {
            throw new NotAuthorizedException("Token verification failed");
        }
    }

    private static UserResponse MapToUserResponse(GoogleJsonWebSignature.Payload payload, string jwtToken, IEnumerable<string> userRoleNames, UserDto user) => new()
    {
        Id = user.Id,
        Email = payload.Email,
        Name = payload.Name,
        ProfilePhoto = payload.Picture,
        JwtToken = jwtToken,
        UserRoleNames = userRoleNames
    };

    private static async Task UpdateUserIfNeeded(IUserRepository userRepository, GoogleJsonWebSignature.Payload payload, UserDto userDto, CancellationToken cancellationToken)
    {
        if (UserNeedsUpdate(payload, userDto))
        {
            await userRepository.UpdateUserAsync(CreateUser(payload, userDto).MapToDto(), cancellationToken);
        }
    }

    private static User CreateUser(GoogleJsonWebSignature.Payload payload, UserDto userDto) => User.Create(new()
    {
        Id = userDto.Id,
        Email = userDto.Email,
        Name = payload.Name,
        Picture = payload.Picture,
        Roles = userDto.Roles
    });

    private static bool UserNeedsUpdate(GoogleJsonWebSignature.Payload payload, UserDto user) => payload.Name != user.Name || payload.Picture != user.Picture;
}

public record AuthenticateWithGoogleRequest : IRequest<UserResponse>
{
    public required string Token { get; init; }
}

public record UserResponse
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string ProfilePhoto { get; init; }
    public required string JwtToken { get; init; }
    public required IEnumerable<string> UserRoleNames { get; init; }
}