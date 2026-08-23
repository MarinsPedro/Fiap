using System.Reflection;
using FiapCloudGames.Database.Migrations.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FiapCloudGames.Database.IntegrationTests;

internal static class DatabaseTestContexts
{
    private const string ConnectionString =
        "Host=localhost;Database=model_only;Username=model;Password=model";

    public static ContextSet Create()
    {
        var contexts = DiscoverContextTypes()
            .Select(CreateDescriptor)
            .OrderBy(item => item.Module, StringComparer.Ordinal)
            .ToArray();

        if (contexts.Length == 0)
        {
            throw new InvalidOperationException(
                "Nenhum DbContext de módulo foi descoberto.");
        }

        return new ContextSet(contexts);
    }

    private static Type[] DiscoverContextTypes() =>
        Directory
            .EnumerateFiles(
                AppContext.BaseDirectory,
                "FiapCloudGames.*.Infrastructure.dll",
                SearchOption.TopDirectoryOnly)
            .Select(Assembly.LoadFrom)
            .Where(assembly =>
                assembly.GetName().Name?.Split('.').Length == 3)
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                type.IsPublic &&
                !type.IsAbstract &&
                typeof(DbContext).IsAssignableFrom(type) &&
                type.Name.EndsWith("DbContext", StringComparison.Ordinal))
            .Distinct()
            .ToArray();

    private static ContextDescriptor CreateDescriptor(Type contextType)
    {
        var module = contextType.Name[..^"DbContext".Length];
        var builderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(
            contextType);
        var builder = Assert.IsAssignableFrom<DbContextOptionsBuilder>(
            Activator.CreateInstance(builderType));
        var configureMethod = typeof(MigrationDbContextOptions).GetMethod(
            $"Configure{module}",
            BindingFlags.Public | BindingFlags.Static);

        Assert.NotNull(configureMethod);
        configureMethod.Invoke(null, [builder, ConnectionString]);

        var context = Assert.IsAssignableFrom<DbContext>(
            Activator.CreateInstance(contextType, builder.Options));
        var relationalOptions = context.GetService<IDbContextOptions>()
            .Extensions
            .OfType<RelationalOptionsExtension>()
            .Single();
        var migrationsAssembly = context.GetService<IMigrationsAssembly>();

        return new ContextDescriptor(
            module,
            context,
            relationalOptions,
            migrationsAssembly);
    }
}

internal sealed class ContextSet(
    IReadOnlyList<ContextDescriptor> items) : IDisposable
{
    public IReadOnlyList<ContextDescriptor> Items { get; } = items;

    public void Dispose()
    {
        foreach (var item in Items)
        {
            item.Context.Dispose();
        }
    }
}

internal sealed record ContextDescriptor(
    string Module,
    DbContext Context,
    RelationalOptionsExtension RelationalOptions,
    IMigrationsAssembly MigrationsAssembly);
