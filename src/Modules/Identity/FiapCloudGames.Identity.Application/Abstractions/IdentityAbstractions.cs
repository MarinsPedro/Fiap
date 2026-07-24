using FiapCloudGames.Identity.Domain.Entities;

namespace FiapCloudGames.Identity.Application.Abstractions;

public interface IIdentityUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}

public sealed record GeneratedToken(string AccessToken, DateTimeOffset ExpiresAtUtc);

public interface ITokenGenerator
{
    GeneratedToken Generate(User user);
}
