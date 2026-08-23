using FiapCloudGames.Identity.Application.Features.Authentication.Login;
using FiapCloudGames.Identity.Application.Features.Users.CreateUser;
using FiapCloudGames.Identity.Application.Features.Users.DeactivateUser;
using FiapCloudGames.Identity.Application.Features.Users.FindUser;
using FiapCloudGames.Identity.Application.Features.Users.GetCurrentUser;
using FiapCloudGames.Identity.Application.Features.Users.GetUser;
using FiapCloudGames.Identity.Application.Features.Users.UpdateUser;
using FiapCloudGames.Identity.Application.Integrations;
using FiapCloudGames.Identity.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateUserService>();
        services.AddScoped<UpdateUserService>();
        services.AddScoped<LoginService>();
        services.AddScoped<FindUserService>();
        services.AddScoped<GetCurrentUserService>();
        services.AddScoped<GetUserService>();
        services.AddScoped<DeactivateUserService>();
        services.AddScoped<IIdentityModule, IdentityModule>();
        return services;
    }
}
