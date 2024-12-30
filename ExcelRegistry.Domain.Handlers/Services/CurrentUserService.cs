using ExcelRegistry.Domain.Handlers.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ExcelRegistry.Domain.Handlers.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public ClaimsPrincipal User => httpContextAccessor.HttpContext?.User ?? new();

    public string GetEmail() => User.FindFirst(ClaimTypes.Email)?.Value ?? throw new ArgumentException("User email can not be retrieved.");

    public string GetName() => User.FindFirst(ClaimTypes.Name)?.Value ?? throw new ArgumentException("User name can not be retrieved.");
}
