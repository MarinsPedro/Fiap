using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Identity.Application.Abstractions.Persistence;
using FiapCloudGames.Identity.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Identity.Application.Features.Users.DeactivateUser;

public sealed class DeactivateUserService(
    IUserRepository users,
    IIdentityUnitOfWork unitOfWork,
    ILogger<DeactivateUserService> logger)
{
    public async Task ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Iniciando desativação do usuário {UserId}.",
            id);

        var user = await users.GetAsync(id, cancellationToken);
        if (user is null)
        {
            logger.LogWarning(
                "Não foi possível desativar: usuário {UserId} não encontrado.",
                id);
            throw AppException.NotFound("Usuário não encontrado.");
        }

        if (!user.IsActive)
        {
            logger.LogWarning(
                "Não foi possível desativar: usuário {UserId} já foi desativado.",
                id);
            throw AppException.BusinessRule("Usuário já desativado.");
        }

        user.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Usuário {UserId} desativado com sucesso.",
            user.Id);
    }
}
