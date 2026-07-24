using FiapCloudGames.Database.Migrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(
    new HostApplicationBuilderSettings
    {
        Args = args,
        ContentRootPath = AppContext.BaseDirectory
    });

var connectionString =
    builder.Configuration.GetConnectionString("Database");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "A connection string 'Database' não foi configurada.");
}

builder.Services
    .AddFluentMigratorCore()
    .ConfigureRunner(runner => runner
        .AddPostgres()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(typeof(Program).Assembly).For.Migrations())
    .AddLogging(logging =>
        logging.AddFluentMigratorConsole());

using var host = builder.Build();
using var scope = host.Services.CreateScope();

scope.ServiceProvider
    .GetRequiredService<IMigrationRunner>()
    .MigrateUp();

await AdminSeeder.SeedAsync(
    connectionString,
    builder.Configuration,
    CancellationToken.None);
