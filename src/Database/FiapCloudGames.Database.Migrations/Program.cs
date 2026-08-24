using FiapCloudGames.Catalog.Infrastructure.Persistence;
using FiapCloudGames.Database.Migrations.Configuration;
using FiapCloudGames.Database.Migrations.Initialization;
using FiapCloudGames.Identity.Infrastructure.Persistence;
using FiapCloudGames.Library.Infrastructure.Persistence;
using FiapCloudGames.Promotions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(
    new HostApplicationBuilderSettings
    {
        Args = args,
        ContentRootPath = AppContext.BaseDirectory
    });

var connectionString = builder.Configuration.GetConnectionString("Database");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "A connection string 'Database' não foi configurada.");
}

await MigrationSchemaInitializer.EnsureInfraSchemaAsync(
    connectionString,
    CancellationToken.None);

builder.Services.AddDbContext<IdentityDbContext>(options =>
    MigrationDbContextOptions.ConfigureIdentityWithAdminSeeding(
        options,
        connectionString,
        builder.Configuration,
        TimeProvider.System));

builder.Services.AddDbContext<CatalogDbContext>(options =>
    MigrationDbContextOptions.ConfigureCatalog(options, connectionString));

builder.Services.AddDbContext<PromotionsDbContext>(options =>
    MigrationDbContextOptions.ConfigurePromotions(options, connectionString));

builder.Services.AddDbContext<LibraryDbContext>(options =>
    MigrationDbContextOptions.ConfigureLibrary(options, connectionString));

using var host = builder.Build();
using var scope = host.Services.CreateScope();

await MigrateAsync<IdentityDbContext>(
    scope.ServiceProvider,
    CancellationToken.None);

await MigrateAsync<CatalogDbContext>(
    scope.ServiceProvider,
    CancellationToken.None);

await MigrateAsync<PromotionsDbContext>(
    scope.ServiceProvider,
    CancellationToken.None);

await MigrateAsync<LibraryDbContext>(
    scope.ServiceProvider,
    CancellationToken.None);

static async Task MigrateAsync<TContext>(
    IServiceProvider serviceProvider,
    CancellationToken cancellationToken)
    where TContext : DbContext
{
    var dbContext = serviceProvider.GetRequiredService<TContext>();
    await dbContext.Database.MigrateAsync(cancellationToken);
}
