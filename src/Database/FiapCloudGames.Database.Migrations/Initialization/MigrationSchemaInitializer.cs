using Npgsql;

namespace FiapCloudGames.Database.Migrations.Initialization;

internal static class MigrationSchemaInitializer
{
    public static async Task EnsureInfraSchemaAsync(
        string connectionString,
        CancellationToken cancellationToken)
    {
        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        await using var command = dataSource.CreateCommand(
            "CREATE SCHEMA IF NOT EXISTS infra;");

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
