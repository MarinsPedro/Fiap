using FiapCloudGames.Catalog.Infrastructure.Persistence;
using FiapCloudGames.Database.Migrations.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FiapCloudGames.Database.Migrations.Factories;

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
