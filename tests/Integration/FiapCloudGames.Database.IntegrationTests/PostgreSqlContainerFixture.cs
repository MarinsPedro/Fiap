using DotNet.Testcontainers.Builders;
using Npgsql;
using Testcontainers.PostgreSql;

namespace FiapCloudGames.Database.IntegrationTests;

public sealed class PostgreSqlContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var dataSource = NpgsqlDataSource.Create(ConnectionString);
        await using var command = dataSource.CreateCommand(
            "CREATE SCHEMA IF NOT EXISTS infra;");
        await command.ExecuteNonQueryAsync();
    }

    public async Task DisposeAsync() =>
        await _container.DisposeAsync();
}
