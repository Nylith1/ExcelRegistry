using ExcelRegistry.Domain.Handlers.Users;
using ExcelRegistry.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExcelRegistryApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost(nameof(AuthenticateWithGoogle))]
    public async Task<UserResponse> AuthenticateWithGoogle([FromBody] AuthenticateWithGoogleRequest request)
    {
        return await mediator.Send(request);
    }

    [Authorize]
    [HttpGet(nameof(GetRefreshedJwtToken))]
    public async Task<GetRefreshedJwtTokenResponse> GetRefreshedJwtToken()
    {
        return await mediator.Send(new GetRefreshedJwtTokenRequest());
    }

    [Authorize]
    [HttpGet(nameof(GetUsers))]
    [Authorize(Roles = $"{Constants.Roles.Admin}")]
    public async Task<IEnumerable<GetUsersResponse>> GetUsers()
    {
        return await mediator.Send(new GetUsersRequest());
    }

    [Authorize]
    [HttpPut(nameof(AddUser))]
    [Authorize(Roles = $"{Constants.Roles.Admin}")]
    public async Task AddUser(AddUserRequest request)
    {
        await mediator.Send(request);
    }

    [Authorize]
    [HttpDelete($"{nameof(RemoveUser)}" + "/{userId}")]
    [Authorize(Roles = $"{Constants.Roles.Admin}")]
    public async Task RemoveUser(string userId)
    {
        await mediator.Send(new RemoveUserRequest() { UserId = userId });
    }

    [Authorize]
    [HttpPatch(nameof(GrantAdminRole))]
    [Authorize(Roles = $"{Constants.Roles.Admin}")]
    public async Task GrantAdminRole(GrantAdminRoleRequest request)
    {
        await mediator.Send(request);
    }

    [Authorize]
    [HttpPatch(nameof(RevokeAdminRole))]
    [Authorize(Roles = $"{Constants.Roles.Admin}")]
    public async Task RevokeAdminRole(RevokeAdminRoleRequest request)
    {
        await mediator.Send(request);
    }
}
