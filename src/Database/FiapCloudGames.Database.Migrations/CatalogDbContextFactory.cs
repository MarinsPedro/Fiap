using FiapCloudGames.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FiapCloudGames.Database.Migrations;

public sealed class CatalogDbContextFactory
    : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>();
        MigrationDbContextOptions.ConfigureCatalog(
            options,
            DesignTimeConnectionString.Resolve());

        return new CatalogDbContext(options.Options);
    }
}
