using FiapCloudGames.Catalog.Domain.Entities;
using FiapCloudGames.Catalog.Infrastructure.Persistence;
using FiapCloudGames.Identity.Domain.Entities;
using FiapCloudGames.Identity.Infrastructure.Persistence;
using FiapCloudGames.Library.Domain.Entities;
using FiapCloudGames.Library.Infrastructure.Persistence;
using FiapCloudGames.Promotions.Domain.Entities;
using FiapCloudGames.Promotions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Database.IntegrationTests;

public sealed class MappingCompatibilityTests
{
    private const string ConnectionString = "Host=localhost;Database=model_only;Username=model;Password=model";

    [Fact]
    public void ContextsShouldMapToOwnedSchemasAndTables()
    {
        using var identity = new IdentityDbContext(
            new DbContextOptionsBuilder<IdentityDbContext>().UseNpgsql(ConnectionString).Options);
        using var catalog = new CatalogDbContext(
            new DbContextOptionsBuilder<CatalogDbContext>().UseNpgsql(ConnectionString).Options);
        using var library = new LibraryDbContext(
            new DbContextOptionsBuilder<LibraryDbContext>().UseNpgsql(ConnectionString).Options);
        using var promotions = new PromotionsDbContext(
            new DbContextOptionsBuilder<PromotionsDbContext>().UseNpgsql(ConnectionString).Options);

        AssertMapping<User>(identity, "identity", "users");
        AssertMapping<Game>(catalog, "catalog", "games");
        AssertMapping<GameLibrary>(library, "library", "game_libraries");
        AssertMapping<LibraryGame>(library, "library", "library_games");
        AssertMapping<Promotion>(promotions, "promotions", "promotions");
        AssertMapping<PromotionGame>(promotions, "promotions", "promotion_games");
    }

    private static void AssertMapping<TEntity>(DbContext context, string schema, string table)
    {
        var entityType = context.Model.FindEntityType(typeof(TEntity));
        Assert.NotNull(entityType);
        Assert.Equal(schema, entityType.GetSchema());
        Assert.Equal(table, entityType.GetTableName());
    }
}
