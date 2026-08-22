using System.Net.Mail;
using System.Security.Cryptography;
using FiapCloudGames.Identity.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace FiapCloudGames.Database.Migrations.Seeding;

internal static class AdminSeeder
{
    private const int Iterations = 100_000;

    public static async Task SeedAsync(
        string connectionString,
        IConfiguration configuration,
        TimeProvider clock,
        CancellationToken cancellationToken)
    {
        var email = configuration["Admin:Email"];
        var password = configuration["Admin:Password"];
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(email) || !MailAddress.TryCreate(email, out var address))
        {
            throw new InvalidOperationException("Configure 'Admin:Email' com um e-mail válido.");
        }

        if (!Password.TryCreate(password, out var validPassword))
        {
            throw new InvalidOperationException(
                $"Configure 'Admin:Password' corretamente. {Password.InvalidMessage}");
        }

        var name = configuration["Admin:Name"]?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            name = "Administrador";
        }

        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        await using var command = dataSource.CreateCommand(
            """
            INSERT INTO identity.users (id, name, email, password_hash, role, is_active, created_at_utc)
            VALUES (@id, @name, @email, @password_hash, 2, TRUE, @created_at_utc)
            ON CONFLICT (email) DO NOTHING;
            """);
        command.Parameters.AddWithValue("id", Guid.NewGuid());
        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("email", address.Address.Trim().ToLowerInvariant());
        command.Parameters.AddWithValue(
            "password_hash",
            Hash(validPassword));
        command.Parameters.AddWithValue(
            "created_at_utc",
            clock.GetUtcNow());
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static string Hash(Password password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password.Value,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            32);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }
}
