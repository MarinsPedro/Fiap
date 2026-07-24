using FiapCloudGames.Catalog.Application.Games;
using FiapCloudGames.Catalog.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateGameService>();
        services.AddScoped<UpdateGameService>();
        services.AddScoped<GetGameService>();
        services.AddScoped<ListGamesService>();
        services.AddScoped<ICatalogModule, CatalogModule>();
        return services;
    }
}
