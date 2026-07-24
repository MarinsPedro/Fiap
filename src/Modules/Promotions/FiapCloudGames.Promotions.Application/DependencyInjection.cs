using FiapCloudGames.Promotions.Application.Promotions;
using FiapCloudGames.Promotions.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Promotions.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPromotionsApplication(this IServiceCollection services)
    {
        services.AddScoped<CreatePromotionService>();
        services.AddScoped<ListActivePromotionsService>();
        services.AddScoped<EndPromotionService>();
        services.AddScoped<GetPromotionalPriceService>();
        services.AddScoped<IPromotionsModule, PromotionsModule>();
        return services;
    }
}
