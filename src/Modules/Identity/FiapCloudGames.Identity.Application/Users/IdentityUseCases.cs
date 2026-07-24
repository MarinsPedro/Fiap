using FiapCloudGames.Identity.Application.Abstractions;
using FiapCloudGames.Identity.Contracts;
using FiapCloudGames.Identity.Domain.Entities;
using FiapCloudGames.Identity.Domain.Enums;
using FiapCloudGames.Identity.Domain.Repositories;
using FiapCloudGames.Identity.Domain.ValueObjects;

namespace FiapCloudGames.Identity.Application.Users;

public sealed record CreateUserInput(string Name, string Email, string Password);
public sealed record CreateUserOutput(Guid Id, string Name, string Email, string Role, bool IsActive);
public sealed record LoginInput(string Email, string Password);
public sealed record LoginOutput(string AccessToken, DateTimeOffset ExpiresAtUtc, UserSummary User);

public sealed class CreateUserService(
    IUserRepository users,
    IIdentityUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher)
{
    public async Task<CreateUserOutput> ExecuteAsync(
        CreateUserInput input,
        CancellationToken cancellationToken)
    {
        if (input.Password.Length < 8)
        {
            throw new ArgumentException("A senha deve ter pelo menos 8 caracteres.", nameof(input));
        }

        var email = Email.Create(input.Email);
        if (await users.ExistsAsync(email, cancellationToken))
        {
            throw new InvalidOperationException("Já existe um usuário com este e-mail.");
        }

        var user = User.Create(input.Name, email, passwordHasher.Hash(input.Password));
        await users.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateUserOutput(
            user.Id,
            user.Name,
            user.Email.Value,
            user.Role.ToString(),
            user.IsActive);
    }
}

public sealed class LoginService(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator)
{
    public async Task<LoginOutput> ExecuteAsync(LoginInput input, CancellationToken cancellationToken)
    {
        var user = await users.GetByEmailAsync(Email.Create(input.Email), cancellationToken);
        if (user is null || !passwordHasher.Verify(input.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("O usuário está inativo.");
        }

        var token = tokenGenerator.Generate(user);
        return new LoginOutput(token.AccessToken, token.ExpiresAtUtc, Map(user));
    }

    private static UserSummary Map(User user) =>
        new(user.Id, user.Name, user.Email.Value, user.Role.ToString(), user.IsActive);
}

public sealed class GetUserService(IUserRepository users)
{
    public async Task<UserSummary?> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await users.GetAsync(id, cancellationToken);
        return user is null
            ? null
            : new UserSummary(user.Id, user.Name, user.Email.Value, user.Role.ToString(), user.IsActive);
    }
}

public sealed class DeactivateUserService(IUserRepository users, IIdentityUnitOfWork unitOfWork)
{
    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await users.GetAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");
        user.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

internal sealed class IdentityModule(GetUserService getUserService) : IIdentityModule
{
    public Task<UserSummary?> GetUserAsync(Guid userId, CancellationToken cancellationToken) =>
        getUserService.ExecuteAsync(userId, cancellationToken);
}
