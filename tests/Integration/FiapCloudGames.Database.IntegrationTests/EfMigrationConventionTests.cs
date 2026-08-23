using FiapCloudGames.Database.Migrations.Configuration;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Database.IntegrationTests;

public sealed class EfMigrationConventionTests
{
    [Fact]
    public void AllMigrations_ShouldBeDiscoverable()
    {
        using var contexts = DatabaseTestContexts.Create();

        foreach (var item in contexts.Items)
        {
            Assert.NotEmpty(item.Context.Database.GetMigrations());
        }
    }

    [Fact]
    public void Models_ShouldHaveNoPendingChanges()
    {
        using var contexts = DatabaseTestContexts.Create();

        foreach (var item in contexts.Items)
        {
            Assert.False(
                item.Context.Database.HasPendingModelChanges(),
                $"O modelo {item.Module} diverge do snapshot de migrations.");
        }
    }

    [Fact]
    public void HistoryTables_ShouldUseExpectedSchemaAndConvention()
    {
        using var contexts = DatabaseTestContexts.Create();

        foreach (var item in contexts.Items)
        {
            Assert.Equal(
                $"__EFMigrationsHistory_{item.Module}",
                item.RelationalOptions.MigrationsHistoryTableName);
            Assert.Equal(
                MigrationDbContextOptions.HistorySchema,
                item.RelationalOptions.MigrationsHistoryTableSchema);
        }
    }

    [Fact]
    public void DbContexts_ShouldUseExpectedMigrationAssembly()
    {
        using var contexts = DatabaseTestContexts.Create();
        var expectedAssembly = typeof(MigrationDbContextOptions).Assembly;

        foreach (var item in contexts.Items)
        {
            Assert.Equal(expectedAssembly, item.MigrationsAssembly.Assembly);
        }
    }
}
