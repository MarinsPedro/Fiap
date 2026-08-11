using FiapCloudGames.Library.Application;
using FiapCloudGames.Library.Application.Abstractions.Persistence;
using FiapCloudGames.Library.Application.Abstractions.Queries;
using FiapCloudGames.Library.Domain.Repositories;
using FiapCloudGames.Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Library.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLibraryInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("A connection string 'Database' não foi configurada.");
        services.AddLibraryApplication();
        services.AddDbContext<LibraryDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IGameLibraryRepository, GameLibraryRepository>();
        services.AddScoped<ILibraryQueries, LibraryQueries>();
        services.AddScoped<ILibraryUnitOfWork>(provider => provider.GetRequiredService<LibraryDbContext>());
        return services;
    }
}
