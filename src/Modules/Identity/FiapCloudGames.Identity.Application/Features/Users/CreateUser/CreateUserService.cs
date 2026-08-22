using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Identity.Application.Abstractions.Persistence;
using FiapCloudGames.Identity.Application.Abstractions.Security;
using FiapCloudGames.Identity.Domain.Entities;
using FiapCloudGames.Identity.Domain.Repositories;
using FiapCloudGames.Identity.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Identity.Application.Features.Users.CreateUser;

public sealed class CreateUserService(
    IUserRepository users,
    IIdentityUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    TimeProvider clock,
    ILogger<CreateUserService> logger)
{
    public async Task<UserResult> ExecuteAsync(
        CreateUserInput input,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Iniciando criação de usuário.");

        var email = Email.Create(input.Email);
        var password = ValidatePassword(input.Password);

        if (await users.ExistsAsync(email, cancellationToken))
        {
            logger.LogInformation(
                "Criação de usuário rejeitada: e-mail já cadastrado.");
            throw AppException.Conflict(
                "Já existe um usuário com este e-mail.");
        }

        var user = User.Create(
            input.Name,
            email,
            passwordHasher.Hash(password),
            clock.GetUtcNow());
        await users.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Usuário {UserId} criado com sucesso.",
            user.Id);

        return IdentityApplicationMappings.ToResult(user);
    }

    private static Password ValidatePassword(string? value)
    {
        if (Password.TryCreate(value, out var password))
        {
            return password;
        }

        throw AppException.Validation(
            [
                new AppError(
                    Password.InvalidMessage,
                    "password")
            ]);
    }
}
