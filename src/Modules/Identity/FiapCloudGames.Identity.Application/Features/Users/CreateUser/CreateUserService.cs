using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Identity.Application.Abstractions.Persistence;
using FiapCloudGames.Identity.Application.Abstractions.Security;
using FiapCloudGames.Identity.Domain.Entities;
using FiapCloudGames.Identity.Domain.Repositories;
using FiapCloudGames.Identity.Domain.ValueObjects;

namespace FiapCloudGames.Identity.Application.Features.Users.CreateUser;

public sealed class CreateUserService(
    IUserRepository users,
    IIdentityUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    TimeProvider clock)
{
    public async Task<UserResult> ExecuteAsync(
        CreateUserInput input,
        CancellationToken cancellationToken)
    {
        var email = Email.Create(input.Email);
        ValidatePassword(input.Password);

        if (await users.ExistsAsync(email, cancellationToken))
        {
            throw AppException.Conflict(
                "Já existe um usuário com este e-mail.");
        }

        var user = User.Create(
            input.Name,
            email,
            passwordHasher.Hash(input.Password),
            clock.GetUtcNow());
        await users.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return IdentityApplicationMappings.ToResult(user);
    }

    private static void ValidatePassword(string? password)
    {
        if (!string.IsNullOrWhiteSpace(password) && password.Length >= 8)
        {
            return;
        }

        throw AppException.Validation(
            new Dictionary<string, string[]>
            {
                ["password"] =
                    ["A senha deve ter pelo menos 8 caracteres."]
            });
    }
}
