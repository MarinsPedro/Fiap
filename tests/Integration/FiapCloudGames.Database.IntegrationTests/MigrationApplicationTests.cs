using FiapCloudGames.Catalog.Infrastructure.Persistence;
using FiapCloudGames.Database.Migrations.Configuration;
using FiapCloudGames.Identity.Infrastructure.Persistence;
using FiapCloudGames.Library.Infrastructure.Persistence;
using FiapCloudGames.Promotions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Database.IntegrationTests;

public sealed class MigrationApplicationTests(PostgreSqlContainerFixture fixture)
    : IClassFixture<PostgreSqlContainerFixture>
{
    [Fact]
    public async Task IdentityMigrationsShouldApplySuccessfully()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>();
        MigrationDbContextOptions.ConfigureIdentity(options, fixture.ConnectionString);
        await using var context = new IdentityDbContext(options.Options);

        await context.Database.MigrateAsync();

        var pending = await context.Database.GetPendingMigrationsAsync();
        Assert.Empty(pending);
    }

    [Fact]
    public async Task CatalogMigrationsShouldApplySuccessfully()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>();
        MigrationDbContextOptions.ConfigureCatalog(options, fixture.ConnectionString);
        await using var context = new CatalogDbContext(options.Options);

        await context.Database.MigrateAsync();

        var pending = await context.Database.GetPendingMigrationsAsync();
        Assert.Empty(pending);
    }

    [Fact]
    public async Task PromotionsMigrationsShouldApplySuccessfully()
    {
        var options = new DbContextOptionsBuilder<PromotionsDbContext>();
        MigrationDbContextOptions.ConfigurePromotions(options, fixture.ConnectionString);
        await using var context = new PromotionsDbContext(options.Options);

        await context.Database.MigrateAsync();

        var pending = await context.Database.GetPendingMigrationsAsync();
        Assert.Empty(pending);
    }

    [Fact]
    public async Task LibraryMigrationsShouldApplySuccessfully()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>();
        MigrationDbContextOptions.ConfigureLibrary(options, fixture.ConnectionString);
        await using var context = new LibraryDbContext(options.Options);

        await context.Database.MigrateAsync();

        var pending = await context.Database.GetPendingMigrationsAsync();
        Assert.Empty(pending);
    }
}
