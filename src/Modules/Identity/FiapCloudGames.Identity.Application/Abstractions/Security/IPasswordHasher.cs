using FiapCloudGames.Identity.Domain.ValueObjects;

namespace FiapCloudGames.Identity.Application.Abstractions.Security;

public interface IPasswordHasher
{
    string Hash(Password password);
    bool Verify(string password, string passwordHash);
}
