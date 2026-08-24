using System.Net.Mail;
using System.Security.Cryptography;
using FiapCloudGames.Identity.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace FiapCloudGames.Database.Migrations.Seeding;

internal static class AdminSeeder
{
    private const int Iterations = 100_000;

    private const string UserTableExistsSql =
        "SELECT to_regclass('identity.users') IS NOT NULL;";

    private const string InsertAdminSql =
        """
        INSERT INTO identity.users (id, name, email, password_hash, role, is_active, created_at_utc)
        VALUES (@id, @name, @email, @password_hash, 2, TRUE, @created_at_utc)
        ON CONFLICT (email) DO NOTHING;
        """;

    public static void Seed(
        string connectionString,
        IConfiguration configuration,
        TimeProvider clock)
    {
        if (!IsRequested(configuration))
        {
            return;
        }

        using var dataSource = NpgsqlDataSource.Create(connectionString);
        using var connection = dataSource.OpenConnection();

        if (!UserTableExists(connection))
        {
            return;
        }

        var admin = ResolveAdmin(configuration);
        using var command = CreateInsertCommand(connection, admin, clock);
        command.ExecuteNonQuery();
    }

    public static async Task SeedAsync(
        string connectionString,
        IConfiguration configuration,
        TimeProvider clock,
        CancellationToken cancellationToken)
    {
        if (!IsRequested(configuration))
        {
            return;
        }

        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        await using var connection =
            await dataSource.OpenConnectionAsync(cancellationToken);

        if (!await UserTableExistsAsync(connection, cancellationToken))
        {
            return;
        }

        var admin = ResolveAdmin(configuration);
        await using var command = CreateInsertCommand(connection, admin, clock);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static bool IsRequested(IConfiguration configuration)
    {
        var email = configuration["Admin:Email"];
        var password = configuration["Admin:Password"];

        return !string.IsNullOrWhiteSpace(email) ||
            !string.IsNullOrWhiteSpace(password);
    }

    private static AdminSeed ResolveAdmin(IConfiguration configuration)
    {
        var email = configuration["Admin:Email"];
        var password = configuration["Admin:Password"];

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

        return new AdminSeed(
            name,
            address.Address.Trim().ToLowerInvariant(),
            Hash(validPassword));
    }

    private static bool UserTableExists(NpgsqlConnection connection)
    {
        using var command = new NpgsqlCommand(UserTableExistsSql, connection);
        return command.ExecuteScalar() is bool exists && exists;
    }

    private static async Task<bool> UserTableExistsAsync(
        NpgsqlConnection connection,
        CancellationToken cancellationToken)
    {
        await using var command =
            new NpgsqlCommand(UserTableExistsSql, connection);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is bool exists && exists;
    }

    private static NpgsqlCommand CreateInsertCommand(
        NpgsqlConnection connection,
        AdminSeed admin,
        TimeProvider clock)
    {
        var command = new NpgsqlCommand(InsertAdminSql, connection);
        command.Parameters.AddWithValue("id", Guid.NewGuid());
        command.Parameters.AddWithValue("name", admin.Name);
        command.Parameters.AddWithValue("email", admin.Email);
        command.Parameters.AddWithValue("password_hash", admin.PasswordHash);
        command.Parameters.AddWithValue(
            "created_at_utc",
            clock.GetUtcNow());

        return command;
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

    private sealed record AdminSeed(
        string Name,
        string Email,
        string PasswordHash);
}
