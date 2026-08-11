using System.Text.Json;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ApiProblemDetailsFactory.Customize;
});
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
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = actionContext =>
        {
            var errors = actionContext.ModelState
                .Where(item => item.Value is { Errors.Count: > 0 })
                .ToDictionary(
                    item => NormalizeFieldName(item.Key),
                    item => item.Value!.Errors
                        .Select(ToValidationMessage)
                        .Distinct(StringComparer.Ordinal)
                        .ToArray(),
                    StringComparer.OrdinalIgnoreCase);

            var result = new BadRequestObjectResult(
                ApiProblemDetailsFactory.CreateValidation(
                    actionContext.HttpContext,
                    errors));

            result.ContentTypes.Add("application/problem+json");
            return result;
        };
    })
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
app.UseStatusCodePages();
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

static string NormalizeFieldName(string fieldName) =>
    string.IsNullOrWhiteSpace(fieldName) || fieldName.StartsWith('$')
        ? fieldName
        : JsonNamingPolicy.CamelCase.ConvertName(fieldName);

static string ToValidationMessage(ModelError error)
{
    if (error.Exception is JsonException)
    {
        return "O JSON enviado é inválido.";
    }

    return string.IsNullOrWhiteSpace(error.ErrorMessage)
        ? "O valor informado é inválido."
        : error.ErrorMessage;
}

public partial class Program;
