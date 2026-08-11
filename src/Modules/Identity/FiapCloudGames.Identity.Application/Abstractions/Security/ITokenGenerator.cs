using FiapCloudGames.Identity.Domain.Entities;

namespace FiapCloudGames.Identity.Application.Abstractions.Security;

public interface ITokenGenerator
{
    GeneratedToken Generate(User user);
}
