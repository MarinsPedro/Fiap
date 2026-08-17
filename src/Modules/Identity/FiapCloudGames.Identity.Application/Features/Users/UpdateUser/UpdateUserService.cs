using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Identity.Application.Abstractions.Persistence;
using FiapCloudGames.Identity.Domain.Repositories;
using FiapCloudGames.Identity.Domain.ValueObjects;

namespace FiapCloudGames.Identity.Application.Features.Users.UpdateUser;

public class UpdateUserService(
    IUserRepository userRepository,
    IIdentityUnitOfWork unitOfWork)
{
    public async Task<UserResult> ExecuteAsync(
        Guid id,
        UpdateUserInput input,
        CancellationToken cancellationToken)
    {
        var email = Email.Create(input.Email);

        if (await userRepository.ExistsEmailWithDifferentIdAsync(id, email, cancellationToken))
        {
            throw AppException.Conflict("Já existe um usuário com este e-mail.");
        }

        var user = await userRepository.GetAsync(id, cancellationToken);

        if(user is null)
        {
            throw AppException.NotFound("Usuário não encontrado.");
        }

        user.ChangeDetails(input.Name, email);        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return IdentityApplicationMappings.ToResult(user);
    }
}
