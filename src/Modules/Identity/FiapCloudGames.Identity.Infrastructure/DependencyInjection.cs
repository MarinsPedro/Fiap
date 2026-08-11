using System.Security.Claims;
using System.Text;
using FiapCloudGames.Identity.Application;
using FiapCloudGames.Identity.Application.Abstractions.Persistence;
using FiapCloudGames.Identity.Application.Abstractions.Security;
using FiapCloudGames.Identity.Domain.Repositories;
using FiapCloudGames.Identity.Infrastructure.Authentication;
using FiapCloudGames.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FiapCloudGames.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException(
                "A connection string 'Database' não foi configurada.");

        var issuer = configuration["Jwt:Issuer"] ?? "FiapCloudGames";
        var audience = configuration["Jwt:Audience"] ?? "FiapCloudGames.Client";
        var key = configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
        {
            throw new InvalidOperationException(
                "Configure 'Jwt:Key' com ao menos 32 caracteres por variável de ambiente ou user-secrets.");
        }

        services.AddIdentityApplication();

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IIdentityUnitOfWork>(provider =>
            provider.GetRequiredService<IdentityDbContext>());

        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddSingleton<ITokenGenerator>(provider =>
            new JwtTokenGenerator(
                issuer,
                audience,
                key,
                provider.GetRequiredService<TimeProvider>()));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,

                        ValidateAudience = true,
                        ValidAudience = audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(key)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1),

                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.NameIdentifier
                    };
            });

        services.AddAuthorization();

        return services;
    }
}
