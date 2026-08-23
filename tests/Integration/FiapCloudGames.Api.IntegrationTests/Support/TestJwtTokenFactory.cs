using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FiapCloudGames.Api.IntegrationTests.Support;

internal static class TestJwtTokenFactory
{
    public static string Create(string role, string? userId = null)
    {
        var now = DateTime.UtcNow;
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(FiapCloudGamesApiFactory.JwtKey)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            FiapCloudGamesApiFactory.JwtIssuer,
            FiapCloudGamesApiFactory.JwtAudience,
            [
                new Claim(
                    ClaimTypes.NameIdentifier,
                    userId ?? Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, role)
            ],
            now.AddMinutes(-1),
            now.AddMinutes(5),
            credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
