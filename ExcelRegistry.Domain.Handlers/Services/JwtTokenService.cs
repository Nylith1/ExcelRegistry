using ExcelRegistry.Domain.Handlers.Services.Interfaces;
using ExcelRegistry.Shared.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExcelRegistry.Domain.Handlers.Services;

public class JwtTokenService(IOptions<JwtOptions> jwtOptions) : IJwtTokenService
{
    private readonly JwtOptions jwtOptions = jwtOptions.Value;

    public string Generate(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
           issuer: jwtOptions.Issuer,
           audience: jwtOptions.Audience,
           claims: claims,
           expires: DateTime.UtcNow.AddHours(jwtOptions.TokenLifetimeInHours),
           signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
