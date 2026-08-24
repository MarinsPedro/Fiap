using Microsoft.Extensions.Configuration;

namespace FiapCloudGames.Database.Migrations.Configuration;

internal static class DesignTimeConnectionString
{
    public static IConfigurationRoot BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();
    }

    public static string Resolve()
    {
        return Resolve(BuildConfiguration());
    }

    public static string Resolve(IConfiguration configuration)
    {
        return configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException(
                "A connection string 'Database' não foi encontrada");
    }
}
