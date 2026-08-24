using FiapCloudGames.Database.Migrations.Configuration;
using FiapCloudGames.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FiapCloudGames.Database.Migrations.Factories;

public sealed class IdentityDbContextFactory
    : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var configuration =
            DesignTimeConnectionString.BuildConfiguration();
        var connectionString =
            DesignTimeConnectionString.Resolve(configuration);
        var options = new DbContextOptionsBuilder<IdentityDbContext>();
        MigrationDbContextOptions.ConfigureIdentityWithAdminSeeding(
            options,
            connectionString,
            configuration,
            TimeProvider.System);

        return new IdentityDbContext(options.Options);
    }
}
