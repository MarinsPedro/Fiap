using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FiapCloudGames.Identity.Application.Abstractions.Security;
using FiapCloudGames.Identity.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace FiapCloudGames.Identity.Infrastructure.Authentication;

internal sealed class JwtTokenGenerator(
    string issuer,
    string audience,
    string key,
    TimeProvider clock) : ITokenGenerator
{
    public GeneratedToken Generate(User user)
    {
        var issuedAt = clock.GetUtcNow();
        var expiresAt = issuedAt.AddHours(2);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email.Value),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, issuedAt.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer, audience, claims, issuedAt.UtcDateTime, expiresAt.UtcDateTime, credentials);
        return new GeneratedToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
