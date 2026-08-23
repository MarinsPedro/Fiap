using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Identity.Application.Features.Users.FindUser;

namespace FiapCloudGames.Identity.Application.Features.Users.GetUser;

public sealed class GetUserService(FindUserService findUser)
{
    public async Task<UserResult> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var user = await findUser.ExecuteAsync(id, cancellationToken);
        return user ?? throw AppException.NotFound("Usuário não encontrado.");
    }
}
