using FiapCloudGames.Database.Migrations.Configuration;
using FiapCloudGames.Promotions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FiapCloudGames.Database.Migrations.Factories;

public sealed class PromotionsDbContextFactory
    : IDesignTimeDbContextFactory<PromotionsDbContext>
{
    public PromotionsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PromotionsDbContext>();
        MigrationDbContextOptions.ConfigurePromotions(
            options,
            DesignTimeConnectionString.Resolve());

        return new PromotionsDbContext(options.Options);
    }
}
