using FiapCloudGames.Promotions.Application.Features.Pricing.GetPromotionalPrice;
using FiapCloudGames.Promotions.Application.Features.Promotions.CreatePromotion;
using FiapCloudGames.Promotions.Application.Features.Promotions.EndPromotion;
using FiapCloudGames.Promotions.Application.Features.Promotions.GetPromotion;
using FiapCloudGames.Promotions.Application.Features.Promotions.ListActivePromotions;
using FiapCloudGames.Promotions.Application.Integrations;
using FiapCloudGames.Promotions.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Promotions.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPromotionsApplication(this IServiceCollection services)
    {
        services.AddScoped<CreatePromotionService>();
        services.AddScoped<GetPromotionService>();
        services.AddScoped<ListActivePromotionsService>();
        services.AddScoped<EndPromotionService>();
        services.AddScoped<GetPromotionalPriceService>();
        services.AddScoped<IPromotionsModule, PromotionsModule>();
        return services;
    }
}
