using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Identity.Application.Abstractions.Security;
using FiapCloudGames.Identity.Application.Features.Users;
using FiapCloudGames.Identity.Domain.Repositories;
using FiapCloudGames.Identity.Domain.ValueObjects;

namespace FiapCloudGames.Identity.Application.Features.Authentication.Login;

public sealed class LoginService(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator)
{
    public async Task<LoginResult> ExecuteAsync(
        LoginInput input,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.Password) ||
            !Email.TryCreate(input.Email, out var email))
        {
            throw InvalidCredentials();
        }

        var user = await users.GetByEmailAsync(email, cancellationToken);
        if (user is null ||
            !passwordHasher.Verify(
                input.Password,
                user.PasswordHash))
        {
            throw InvalidCredentials();
        }

        if (!user.IsActive)
        {
            throw AppException.Forbidden(
                "O usuário está inativo.");
        }

        var token = tokenGenerator.Generate(user);

        return new LoginResult(
            token.AccessToken,
            token.ExpiresAtUtc,
            IdentityApplicationMappings.ToResult(user));
    }

    private static AppException InvalidCredentials() =>
        AppException.Authentication(
            "E-mail ou senha inválidos.");
}
