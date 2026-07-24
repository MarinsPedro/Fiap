using System.Text.Json.Serialization;
using FiapCloudGames.Api.Middlewares;
using FiapCloudGames.Catalog.Infrastructure;
using FiapCloudGames.Catalog.Presentation;
using FiapCloudGames.Identity.Infrastructure;
using FiapCloudGames.Identity.Presentation;
using FiapCloudGames.Library.Infrastructure;
using FiapCloudGames.Library.Presentation;
using FiapCloudGames.Promotions.Infrastructure;
using FiapCloudGames.Promotions.Presentation;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Informe somente o token JWT, sem escrever Bearer."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("bearer", document)] = []
        });
});

builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var origins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        if (origins.Length > 0)
        {
            policy
                .WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()))
    .AddApplicationPart(typeof(IdentityPresentationAssemblyReference).Assembly)
    .AddApplicationPart(typeof(CatalogPresentationAssemblyReference).Assembly)
    .AddApplicationPart(typeof(LibraryPresentationAssemblyReference).Assembly)
    .AddApplicationPart(typeof(PromotionsPresentationAssemblyReference).Assembly);

builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddCatalogInfrastructure(builder.Configuration);
builder.Services.AddPromotionsInfrastructure(builder.Configuration);
builder.Services.AddLibraryInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.EnablePersistAuthorization();
    });
}

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();

public partial class Program;
