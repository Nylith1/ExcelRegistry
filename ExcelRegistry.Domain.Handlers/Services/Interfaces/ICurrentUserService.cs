using System.Security.Claims;

namespace ExcelRegistry.Domain.Handlers.Services.Interfaces;

public interface ICurrentUserService
{
    ClaimsPrincipal User { get; }

    string GetEmail();
    string GetName();
}
