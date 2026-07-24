using System.Reflection;
using FiapCloudGames.Catalog.Domain.Entities;
using FiapCloudGames.Database.Migrations.Migrations;
using FiapCloudGames.Identity.Domain.Entities;
using FiapCloudGames.Library.Domain.Entities;
using FiapCloudGames.Promotions.Domain.Entities;
using NetArchTest.Rules;

namespace FiapCloudGames.ArchitectureTests;

public sealed class ArchitectureRulesTests
{
    public static IEnumerable<object[]> DomainAssemblies()
    {
        yield return [typeof(User).Assembly];
        yield return [typeof(Game).Assembly];
        yield return [typeof(GameLibrary).Assembly];
        yield return [typeof(Promotion).Assembly];
    }

    [Theory]
    [MemberData(nameof(DomainAssemblies))]
    public void DomainShouldNotReferenceFrameworkOrInfrastructure(Assembly assembly)
    {
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.AspNetCore", "Microsoft.EntityFrameworkCore", ".Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join(Environment.NewLine, result.FailingTypeNames ?? []));
    }

    [Fact]
    public void LibraryApplicationShouldUseOnlyOtherModuleContracts()
    {
        var assembly = typeof(FiapCloudGames.Library.Application.DependencyInjection).Assembly;
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "FiapCloudGames.Identity.Application",
                "FiapCloudGames.Identity.Infrastructure",
                "FiapCloudGames.Catalog.Application",
                "FiapCloudGames.Catalog.Infrastructure",
                "FiapCloudGames.Promotions.Application",
                "FiapCloudGames.Promotions.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join(Environment.NewLine, result.FailingTypeNames ?? []));
    }

    [Fact]
    public void MigrationsShouldNotReferenceModules()
    {
        var result = Types.InAssembly(typeof(InitialDatabase).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "FiapCloudGames.Identity",
                "FiapCloudGames.Catalog",
                "FiapCloudGames.Library",
                "FiapCloudGames.Promotions")
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join(Environment.NewLine, result.FailingTypeNames ?? []));
    }
}
