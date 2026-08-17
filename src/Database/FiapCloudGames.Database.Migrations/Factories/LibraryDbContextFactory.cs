using FiapCloudGames.Database.Migrations.Configuration;
using FiapCloudGames.Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FiapCloudGames.Database.Migrations.Factories;

public sealed class LibraryDbContextFactory
    : IDesignTimeDbContextFactory<LibraryDbContext>
{
    public LibraryDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>();
        MigrationDbContextOptions.ConfigureLibrary(
            options,
            DesignTimeConnectionString.Resolve());

        return new LibraryDbContext(options.Options);
    }
}
