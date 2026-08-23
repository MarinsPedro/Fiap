using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using FiapCloudGames.Api.IntegrationTests;

namespace FiapCloudGames.Api.IntegrationTests.Host;

public sealed class LoggingConfigurationTests
    : IClassFixture<FiapCloudGamesApiFactory>
{
    private readonly FiapCloudGamesApiFactory _factory;

    public LoggingConfigurationTests(FiapCloudGamesApiFactory factory) =>
        _factory = factory;

    [Fact]
    public void LoggingShouldTrackTraceAndSpanIdentifiers()
    {
        var options = _factory.Services
            .GetRequiredService<IOptions<LoggerFactoryOptions>>()
            .Value;

        Assert.True(
            options.ActivityTrackingOptions.HasFlag(
                ActivityTrackingOptions.TraceId));
        Assert.True(
            options.ActivityTrackingOptions.HasFlag(
                ActivityTrackingOptions.SpanId));
    }
}
