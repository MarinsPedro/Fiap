using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FiapCloudGames.Api.IntegrationTests;

public sealed class FiapCloudGamesApiFactory
    : WebApplicationFactory<Program>
{
    public FiapCloudGamesApiFactory()
    {
        Environment.SetEnvironmentVariable(
            "ConnectionStrings__Database",
            "Host=localhost;Database=tests;Username=tests;Password=tests");
        Environment.SetEnvironmentVariable(
            "Jwt__Key",
            "integration-tests-only-key-with-more-than-32-characters");
        Environment.SetEnvironmentVariable(
            "Jwt__Issuer",
            "FiapCloudGames.Tests");
        Environment.SetEnvironmentVariable(
            "Jwt__Audience",
            "FiapCloudGames.Tests");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}
