using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Identity.Application.Abstractions.Security;
using FiapCloudGames.Identity.Application.Features.Users;
using FiapCloudGames.Identity.Domain.Repositories;
using FiapCloudGames.Identity.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Identity.Application.Features.Authentication.Login;

public sealed class LoginService(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator,
    ILogger<LoginService> logger)
{
    public async Task<LoginResult> ExecuteAsync(
        LoginInput input,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Iniciando autenticação de usuário.");

        if (string.IsNullOrWhiteSpace(input.Password) ||
            !Email.TryCreate(input.Email, out var email))
        {
            logger.LogInformation(
                "Falha de autenticação por credenciais inválidas.");
            throw InvalidCredentials();
        }

        var user = await users.GetByEmailAsync(email, cancellationToken);
        if (user is null ||
            !passwordHasher.Verify(
                input.Password,
                user.PasswordHash))
        {
            logger.LogInformation(
                "Falha de autenticação por credenciais inválidas.");
            throw InvalidCredentials();
        }

        if (!user.IsActive)
        {
            logger.LogInformation(
                "Falha de autenticação: usuário {UserId} está inativo.",
                user.Id);
            throw AppException.Forbidden(
                "O usuário está inativo.");
        }

        var token = tokenGenerator.Generate(user);

        logger.LogInformation(
            "Usuário {UserId} autenticado com sucesso.",
            user.Id);

        return new LoginResult(
            token.AccessToken,
            token.ExpiresAtUtc,
            IdentityApplicationMappings.ToResult(user));
    }

    private static AppException InvalidCredentials() =>
        AppException.Authentication(
            "E-mail ou senha inválidos.");
}
