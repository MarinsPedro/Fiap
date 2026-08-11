using FiapCloudGames.Promotions.Application;
using FiapCloudGames.Promotions.Application.Abstractions.Persistence;
using FiapCloudGames.Promotions.Domain.Repositories;
using FiapCloudGames.Promotions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Promotions.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPromotionsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("A connection string 'Database' não foi configurada.");
        services.AddPromotionsApplication();
        services.AddDbContext<PromotionsDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IPromotionRepository, PromotionRepository>();
        services.AddScoped<IPromotionsUnitOfWork>(provider => provider.GetRequiredService<PromotionsDbContext>());
        return services;
    }
}
