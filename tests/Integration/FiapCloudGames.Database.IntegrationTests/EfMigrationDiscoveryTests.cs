using FiapCloudGames.Catalog.Infrastructure.Persistence;
using FiapCloudGames.Database.Migrations;
using FiapCloudGames.Identity.Infrastructure.Persistence;
using FiapCloudGames.Library.Infrastructure.Persistence;
using FiapCloudGames.Promotions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FiapCloudGames.Database.IntegrationTests;

public sealed class EfMigrationDiscoveryTests
{
    private const string ConnectionString =
        "Host=localhost;Database=model_only;Username=model;Password=model";

    [Fact]
    public void ContextsShouldDiscoverTheirCentralizedEfMigrations()
    {
        using var identity = CreateIdentityContext();
        using var catalog = CreateCatalogContext();
        using var promotions = CreatePromotionsContext();
        using var library = CreateLibraryContext();

        AssertMigration(identity, "InitialIdentity");
        AssertMigration(catalog, "InitialCatalog");
        AssertMigration(promotions, "InitialPromotions");
        AssertMigration(library, "InitialLibrary");
    }

    [Fact]
    public void ContextModelsShouldMatchTheirMigrationSnapshots()
    {
        using var identity = CreateIdentityContext();
        using var catalog = CreateCatalogContext();
        using var promotions = CreatePromotionsContext();
        using var library = CreateLibraryContext();

        Assert.False(identity.Database.HasPendingModelChanges());
        Assert.False(catalog.Database.HasPendingModelChanges());
        Assert.False(promotions.Database.HasPendingModelChanges());
        Assert.False(library.Database.HasPendingModelChanges());
    }

    [Fact]
    public void ContextsShouldKeepMigrationHistoryOutsidePublicSchema()
    {
        using var identity = CreateIdentityContext();
        using var catalog = CreateCatalogContext();
        using var promotions = CreatePromotionsContext();
        using var library = CreateLibraryContext();

        AssertHistoryTable(
            identity,
            MigrationDbContextOptions.IdentityHistoryTable);
        AssertHistoryTable(
            catalog,
            MigrationDbContextOptions.CatalogHistoryTable);
        AssertHistoryTable(
            promotions,
            MigrationDbContextOptions.PromotionsHistoryTable);
        AssertHistoryTable(
            library,
            MigrationDbContextOptions.LibraryHistoryTable);
    }

    private static IdentityDbContext CreateIdentityContext()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>();
        MigrationDbContextOptions.ConfigureIdentity(options, ConnectionString);
        return new IdentityDbContext(options.Options);
    }

    private static CatalogDbContext CreateCatalogContext()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>();
        MigrationDbContextOptions.ConfigureCatalog(options, ConnectionString);
        return new CatalogDbContext(options.Options);
    }

    private static PromotionsDbContext CreatePromotionsContext()
    {
        var options = new DbContextOptionsBuilder<PromotionsDbContext>();
        MigrationDbContextOptions.ConfigurePromotions(
            options,
            ConnectionString);
        return new PromotionsDbContext(options.Options);
    }

    private static LibraryDbContext CreateLibraryContext()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>();
        MigrationDbContextOptions.ConfigureLibrary(options, ConnectionString);
        return new LibraryDbContext(options.Options);
    }

    private static void AssertMigration(
        DbContext context,
        string expectedName)
    {
        var migrations = context.Database.GetMigrations();
        Assert.Contains(
            migrations,
            migration => migration.EndsWith(
                $"_{expectedName}",
                StringComparison.Ordinal));
    }

    private static void AssertHistoryTable(
        DbContext context,
        string expectedTable)
    {
        var relationalOptions = context.GetService<IDbContextOptions>()
            .Extensions
            .OfType<RelationalOptionsExtension>()
            .Single();

        Assert.Equal(
            expectedTable,
            relationalOptions.MigrationsHistoryTableName);
        Assert.Equal(
            MigrationDbContextOptions.HistorySchema,
            relationalOptions.MigrationsHistoryTableSchema);
    }
}
