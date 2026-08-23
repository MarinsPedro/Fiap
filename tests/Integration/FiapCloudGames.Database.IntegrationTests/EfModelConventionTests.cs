using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FiapCloudGames.Database.IntegrationTests;

public sealed class EfModelConventionTests
{
    [Fact]
    public void Entities_ShouldUseOwnedSchemaAndNamingConvention()
    {
        using var contexts = DatabaseTestContexts.Create();

        foreach (var item in contexts.Items)
        {
            var expectedSchema = item.Module.ToLowerInvariant();
            var entities = item.Context.Model.GetEntityTypes().ToArray();
            Assert.NotEmpty(entities);

            foreach (var entity in entities)
            {
                var table = Assert.IsType<string>(entity.GetTableName());
                Assert.Equal(expectedSchema, entity.GetSchema());
                Assert.True(
                    IsLowerSnakeCase(table),
                    $"A tabela {table} não segue lower_snake_case.");

                var storeObject = StoreObjectIdentifier.Table(
                    table,
                    entity.GetSchema());

                foreach (var property in entity.GetProperties())
                {
                    var column = property.GetColumnName(storeObject);
                    Assert.False(string.IsNullOrWhiteSpace(column));
                    Assert.True(
                        IsLowerSnakeCase(column!),
                        $"A coluna {table}.{column} não segue lower_snake_case.");
                }
            }
        }
    }

    [Fact]
    public void Entities_ShouldHavePrimaryKeys()
    {
        using var contexts = DatabaseTestContexts.Create();

        foreach (var item in contexts.Items)
        {
            foreach (var entity in item.Context.Model.GetEntityTypes())
            {
                Assert.True(
                    entity.FindPrimaryKey() is not null,
                    $"{entity.DisplayName()} não possui chave primária.");
            }
        }
    }

    [Fact]
    public void DecimalProperties_ShouldHaveExplicitPrecision()
    {
        using var contexts = DatabaseTestContexts.Create();
        var decimalProperties = contexts.Items
            .SelectMany(item => item.Context.Model.GetEntityTypes())
            .SelectMany(entity => entity.GetProperties())
            .Where(IsStoredAsDecimal)
            .ToArray();
        Assert.NotEmpty(decimalProperties);

        foreach (var property in decimalProperties)
        {
            Assert.NotNull(property.GetPrecision());
            Assert.NotNull(property.GetScale());
        }
    }

    [Fact]
    public void DbContexts_ShouldMapOnlyTheirOwnModuleEntities()
    {
        using var contexts = DatabaseTestContexts.Create();

        foreach (var item in contexts.Items)
        {
            var expectedPrefix = $"FiapCloudGames.{item.Module}.Domain";

            foreach (var entity in item.Context.Model.GetEntityTypes())
            {
                Assert.StartsWith(
                    expectedPrefix,
                    entity.ClrType.Namespace,
                    StringComparison.Ordinal);
            }
        }
    }

    private static bool IsStoredAsDecimal(IReadOnlyProperty property)
    {
        var providerType = property.GetValueConverter()?.ProviderClrType ??
                           property.ClrType;
        return providerType == typeof(decimal);
    }

    private static bool IsLowerSnakeCase(string value) =>
        value.Length > 0 &&
        char.IsLower(value[0]) &&
        value.All(character =>
            char.IsLower(character) ||
            char.IsDigit(character) ||
            character == '_');
}
