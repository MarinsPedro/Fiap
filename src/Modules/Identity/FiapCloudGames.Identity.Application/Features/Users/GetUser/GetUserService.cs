using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Identity.Application.Features.Users;
using FiapCloudGames.Identity.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Identity.Application.Features.Users.GetUser;

public sealed class GetUserService(
    IUserRepository users,
    ILogger<GetUserService> logger)
{
    public async Task<UserResult> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        logger.LogDebug(
            "Consultando usuário {UserId}.",
            id);

        var user = await users.GetAsync(id, cancellationToken);

        if (user is null)
        {
            logger.LogDebug(
                "Usuário {UserId} não encontrado.",
                id);
            throw AppException.NotFound("Usuário não encontrado.");
        }

        return IdentityApplicationMappings.ToResult(user);
    }
}
