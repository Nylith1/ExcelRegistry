using System.Security.Claims;

namespace ExcelRegistry.Domain.Handlers.Services.Interfaces;

public interface IJwtTokenService
{
    string Generate(IEnumerable<Claim> claims);
}