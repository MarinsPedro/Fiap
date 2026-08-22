using System.Text.Json.Serialization;
using FiapCloudGames.Api.Authentication;
using FiapCloudGames.Api.Configuration;
using FiapCloudGames.Api.Middlewares;
using FiapCloudGames.Application.Common.Authentication;
using FiapCloudGames.Catalog.Infrastructure;
using FiapCloudGames.Catalog.Presentation;
using FiapCloudGames.Identity.Infrastructure;
using FiapCloudGames.Identity.Presentation;
using FiapCloudGames.Library.Infrastructure;
using FiapCloudGames.Library.Presentation;
using FiapCloudGames.Presentation.Common.Errors;
using FiapCloudGames.Promotions.Infrastructure;
using FiapCloudGames.Promotions.Presentation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.Configure(options =>
    options.ActivityTrackingOptions =
        ActivityTrackingOptions.TraceId |
        ActivityTrackingOptions.SpanId);
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = true;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff zzz ";
});

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var contactSite = "http://www.fiapcloudgame.com.br";

    options.SwaggerDoc(
           "v1",
           new OpenApiInfo
           {
               Title = "FiapCloudGame.Api",
               Version = "v1",
               Description = "Web Api para Fiap Cloud Game.",
               Contact = new OpenApiContact
               {
                   Name = "www.fiapcloudgame.com.br",
                   Email = "contato@fiapcloudgame.com.br",
                   Url = new Uri(contactSite)
               }
           });

    // Adiciona os comentários XML de todos os assemblies do projeto
    var assemblies = new[]
    {
        typeof(IdentityPresentationAssemblyReference).Assembly,
        typeof(CatalogPresentationAssemblyReference).Assembly,
        typeof(LibraryPresentationAssemblyReference).Assembly,
        typeof(PromotionsPresentationAssemblyReference).Assembly
    };

    // Adiciona os comentários XML de cada assembly, se o arquivo XML existir
    foreach (var assembly in assemblies)
    {
        var xmlFile = $"{assembly.GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        if(!File.Exists(xmlPath))
            throw new FileNotFoundException($"O arquivo XML de documentação '{xmlFile}' não foi encontrado. Certifique-se de que a tag <GenerateDocumentationFile>True</GenerateDocumentationFile> está configurada como True no {assembly.ManifestModule.Name.Replace(".dll", ".csproj")}.");

        options.IncludeXmlComments(xmlPath);
    }

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
        options.ConfigureProblemDetailsResponses())
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

app.UseMiddleware<ClientErrorLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStatusCodePages(async (StatusCodeContext statusCodeContext) =>
{
    var context = statusCodeContext.HttpContext;
    var problemDetails = ApiProblemDetailsFactory.CreateStatusCode(
        context,
        context.Response.StatusCode);

    await context.Response.WriteAsJsonAsync(
        problemDetails,
        options: null,
        contentType: ApiProblemDetailsContentTypes.Json,
        cancellationToken: context.RequestAborted);
});
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

await app.RunAsync();

public partial class Program;
