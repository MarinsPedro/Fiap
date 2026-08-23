using FiapCloudGames.Api.IntegrationTests.Support;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Api.IntegrationTests;

public sealed class FiapCloudGamesApiFactory
    : WebApplicationFactory<Program>
{
    internal const string JwtKey =
        "integration-tests-only-key-with-more-than-32-characters";
    internal const string JwtIssuer = "FiapCloudGames.Tests";
    internal const string JwtAudience = "FiapCloudGames.Tests";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseEnvironment("Testing")
            .UseSetting(
                "ConnectionStrings:Database",
                "Host=localhost;Database=tests;Username=tests;Password=tests")
            .UseSetting("Jwt:Key", JwtKey)
            .UseSetting("Jwt:Issuer", JwtIssuer)
            .UseSetting("Jwt:Audience", JwtAudience);
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.ConfigureServices(services =>
        {
            services
                .AddControllers()
                .AddApplicationPart(typeof(TestErrorsController).Assembly);
        });
    }
}
