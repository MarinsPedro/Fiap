using FiapCloudGames.Library.Application.Games;
using FiapCloudGames.Library.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Library.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddLibraryApplication(this IServiceCollection services)
    {
        services.AddScoped<AcquireGameService>();
        services.AddScoped<GetLibraryService>();
        services.AddScoped<ILibraryModule, LibraryModule>();
        return services;
    }
}
