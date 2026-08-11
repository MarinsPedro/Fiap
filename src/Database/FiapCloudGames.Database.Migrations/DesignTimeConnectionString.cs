using Microsoft.Extensions.Configuration;

namespace FiapCloudGames.Database.Migrations;

internal static class DesignTimeConnectionString
{
    public static string Resolve()
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        return configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException(
                "A connection string 'Database' não foi encontrada");
    }
}
