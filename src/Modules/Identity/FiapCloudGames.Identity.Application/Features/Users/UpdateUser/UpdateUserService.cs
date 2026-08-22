using FiapCloudGames.Application.Common.Authentication;
using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Identity.Application.Abstractions.Persistence;
using FiapCloudGames.Identity.Domain.Repositories;
using FiapCloudGames.Identity.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Identity.Application.Features.Users.UpdateUser;

public class UpdateUserService(
    ICurrentUserContext currentUser,
    IUserRepository userRepository,
    IIdentityUnitOfWork unitOfWork,
    ILogger<UpdateUserService> logger)
{
    public async Task<UserResult> ExecuteAsync(
        UpdateUserInput input,
        CancellationToken cancellationToken)
    {
        var id = currentUser.GetRequiredUserId();

        logger.LogInformation(
            "Iniciando atualização do usuário {UserId}.",
            id);

        var email = Email.Create(input.Email);

        if (await userRepository.ExistsEmailWithDifferentIdAsync(id, email, cancellationToken))
        {
            logger.LogInformation(
                "Não foi possível atualizar o usuário {UserId}: e-mail já cadastrado.",
                id);
            throw AppException.Conflict(
                "Já existe um usuário com este e-mail.");
        }

        var user = await userRepository.GetAsync(id, cancellationToken);

        if (user is null)
        {
            logger.LogInformation(
                "Não foi possível atualizar: usuário {UserId} não encontrado.",
                id);
            throw AppException.NotFound(
                "Usuário não encontrado.");
        }

        user.ChangeDetails(input.Name, email);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Usuário {UserId} atualizado com sucesso.",
            user.Id);

        return IdentityApplicationMappings.ToResult(user);
    }
}
