using FiapCloudGames.Identity.Application.Users;
using FiapCloudGames.Identity.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateUserService>();
        services.AddScoped<LoginService>();
        services.AddScoped<GetUserService>();
        services.AddScoped<DeactivateUserService>();
        services.AddScoped<IIdentityModule, IdentityModule>();
        return services;
    }
}
