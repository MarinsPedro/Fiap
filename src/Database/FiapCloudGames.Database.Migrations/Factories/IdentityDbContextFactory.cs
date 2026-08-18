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
        var options = new DbContextOptionsBuilder<IdentityDbContext>();
        MigrationDbContextOptions.ConfigureIdentity(
            options,
            DesignTimeConnectionString.Resolve());

        return new IdentityDbContext(options.Options);
    }
}
