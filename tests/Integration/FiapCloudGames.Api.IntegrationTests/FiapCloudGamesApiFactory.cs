using FiapCloudGames.Catalog.Domain.Entities;
using FiapCloudGames.Catalog.Domain.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Api.IntegrationTests;

public sealed class FiapCloudGamesApiFactory
    : WebApplicationFactory<Program>
{
    internal const string JwtKey =
        "integration-tests-only-key-with-more-than-32-characters";
    internal const string JwtIssuer = "FiapCloudGames.Tests";
    internal const string JwtAudience = "FiapCloudGames.Tests";

    private readonly TestGameRepository _games = new();

    public Guid InactiveGameId => _games.InactiveGameId;

    public FiapCloudGamesApiFactory()
    {
        Environment.SetEnvironmentVariable(
            "ConnectionStrings__Database",
            "Host=localhost;Database=tests;Username=tests;Password=tests");
        Environment.SetEnvironmentVariable(
            "Jwt__Key",
            JwtKey);
        Environment.SetEnvironmentVariable(
            "Jwt__Issuer",
            JwtIssuer);
        Environment.SetEnvironmentVariable(
            "Jwt__Audience",
            JwtAudience);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IGameRepository>();
            services.AddSingleton<IGameRepository>(_games);
        });
    }

    private sealed class TestGameRepository : IGameRepository
    {
        private readonly Game _inactiveGame;

        public TestGameRepository()
        {
            _inactiveGame = Game.Create(
                "Jogo inativo",
                "Jogo usado pelos testes de integração.",
                "Testes",
                10m,
                DateTimeOffset.UtcNow);
            _inactiveGame.Deactivate();
        }

        public Guid InactiveGameId => _inactiveGame.Id;

        public Task AddAsync(
            Game game,
            CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task<Game?> GetAsync(
            Guid id,
            CancellationToken cancellationToken) =>
            Task.FromResult<Game?>(
                id == InactiveGameId
                    ? _inactiveGame
                    : null);

        public Task<IReadOnlyList<Game>> ListByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<Game>>(
                ids.Contains(InactiveGameId)
                    ? [_inactiveGame]
                    : []);

        public Task<IReadOnlyList<Game>> ListAsync(
            bool onlyActive,
            CancellationToken cancellationToken) =>
            throw new InvalidOperationException(
                "Falha técnica simulada do banco de dados.");
    }
}
