using FiapCloudGames.Database.Migrations.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

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

    [Fact]
    public void OnlyIdentityOptions_ShouldConfigureAdminSeeders()
    {
        var configuration = new ConfigurationBuilder().Build();
        var identityOptions = new DbContextOptionsBuilder();
        var catalogOptions = new DbContextOptionsBuilder();

        MigrationDbContextOptions.ConfigureIdentityWithAdminSeeding(
            identityOptions,
            "Host=localhost;Database=model_only;Username=model;Password=model",
            configuration,
            TimeProvider.System);
        MigrationDbContextOptions.ConfigureCatalog(
            catalogOptions,
            "Host=localhost;Database=model_only;Username=model;Password=model");

        var identityCoreOptions = identityOptions.Options.Extensions
            .OfType<CoreOptionsExtension>()
            .Single();
        var catalogCoreOptions = catalogOptions.Options.Extensions
            .OfType<CoreOptionsExtension>()
            .Single();

        Assert.NotNull(identityCoreOptions.Seeder);
        Assert.NotNull(identityCoreOptions.AsyncSeeder);
        Assert.Null(catalogCoreOptions.Seeder);
        Assert.Null(catalogCoreOptions.AsyncSeeder);
    }
}
