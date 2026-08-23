using FiapCloudGames.Catalog.Application.Features.Games.CreateGame;
using FiapCloudGames.Catalog.Application.Features.Games.DeactivateGame;
using FiapCloudGames.Catalog.Application.Features.Games.FindGame;
using FiapCloudGames.Catalog.Application.Features.Games.FindGames;
using FiapCloudGames.Catalog.Application.Features.Games.GetGame;
using FiapCloudGames.Catalog.Application.Features.Games.ListGames;
using FiapCloudGames.Catalog.Application.Features.Games.UpdateGame;
using FiapCloudGames.Catalog.Application.Integrations;
using FiapCloudGames.Catalog.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateGameService>();
        services.AddScoped<UpdateGameService>();
        services.AddScoped<FindGameService>();
        services.AddScoped<FindGamesService>();
        services.AddScoped<GetGameService>();
        services.AddScoped<ListGamesService>();
        services.AddScoped<DeactivateGameService>();
        services.AddScoped<ICatalogModule, CatalogModule>();
        return services;
    }
}
