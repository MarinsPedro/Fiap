using System.Reflection;
using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Catalog.Domain.Entities;
using FiapCloudGames.Database.Migrations.Configuration;
using FiapCloudGames.Domain.Common;
using FiapCloudGames.Identity.Contracts;
using FiapCloudGames.Identity.Domain.Entities;
using FiapCloudGames.Library.Contracts;
using FiapCloudGames.Library.Domain.Entities;
using FiapCloudGames.Promotions.Contracts;
using FiapCloudGames.Promotions.Domain.Entities;
using NetArchTest.Rules;

namespace FiapCloudGames.ArchitectureTests;

public sealed class ArchitectureRulesTests
{
    private static readonly string[] ApplicationNamespaces =
    [
        "FiapCloudGames.Identity.Application",
        "FiapCloudGames.Catalog.Application",
        "FiapCloudGames.Library.Application",
        "FiapCloudGames.Promotions.Application"
    ];

    private static readonly string[] DomainNamespaces =
    [
        "FiapCloudGames.Identity.Domain",
        "FiapCloudGames.Catalog.Domain",
        "FiapCloudGames.Library.Domain",
        "FiapCloudGames.Promotions.Domain"
    ];

    private static readonly string[] InfrastructureNamespaces =
    [
        "FiapCloudGames.Identity.Infrastructure",
        "FiapCloudGames.Catalog.Infrastructure",
        "FiapCloudGames.Library.Infrastructure",
        "FiapCloudGames.Promotions.Infrastructure"
    ];

    private static readonly string[] PresentationNamespaces =
    [
        "FiapCloudGames.Identity.Presentation",
        "FiapCloudGames.Catalog.Presentation",
        "FiapCloudGames.Library.Presentation",
        "FiapCloudGames.Promotions.Presentation",
        "FiapCloudGames.Presentation.Common"
    ];

    private static readonly string[] ContractNamespaces =
    [
        "FiapCloudGames.Identity.Contracts",
        "FiapCloudGames.Catalog.Contracts",
        "FiapCloudGames.Library.Contracts",
        "FiapCloudGames.Promotions.Contracts"
    ];

    public static IEnumerable<object[]> DomainAssemblies()
    {
        yield return [typeof(User).Assembly];
        yield return [typeof(Game).Assembly];
        yield return [typeof(GameLibrary).Assembly];
        yield return [typeof(Promotion).Assembly];
    }

    public static IEnumerable<object[]> ApplicationAssemblies()
    {
        yield return
            [typeof(Identity.Application.DependencyInjection).Assembly];
        yield return
            [typeof(Catalog.Application.DependencyInjection).Assembly];
        yield return
            [typeof(Library.Application.DependencyInjection).Assembly];
        yield return
            [typeof(Promotions.Application.DependencyInjection).Assembly];
    }

    public static IEnumerable<object[]> InfrastructureAssemblies()
    {
        yield return
            [typeof(Identity.Infrastructure.DependencyInjection).Assembly];
        yield return
            [typeof(Catalog.Infrastructure.DependencyInjection).Assembly];
        yield return
            [typeof(Library.Infrastructure.DependencyInjection).Assembly];
        yield return
            [typeof(Promotions.Infrastructure.DependencyInjection).Assembly];
    }

    public static IEnumerable<object[]> PresentationAssemblies()
    {
        yield return
            [typeof(Identity.Presentation.IdentityPresentationAssemblyReference).Assembly];
        yield return
            [typeof(Catalog.Presentation.CatalogPresentationAssemblyReference).Assembly];
        yield return
            [typeof(Library.Presentation.LibraryPresentationAssemblyReference).Assembly];
        yield return
            [typeof(Promotions.Presentation.PromotionsPresentationAssemblyReference).Assembly];
        yield return
            [typeof(Presentation.Common.PresentationCommonAssemblyReference).Assembly];
    }

    public static IEnumerable<object[]> ContractAssemblies()
    {
        yield return [typeof(UserSnapshot).Assembly];
        yield return [typeof(GameSnapshot).Assembly];
        yield return [typeof(UserLibrarySnapshot).Assembly];
        yield return [typeof(PriceQuoteSnapshot).Assembly];
    }

    [Theory]
    [MemberData(nameof(DomainAssemblies))]
    public void DomainShouldBeFrameworkAndLayerIndependent(
        Assembly assembly)
    {
        var forbiddenDependencies = ApplicationNamespaces
            .Concat(InfrastructureNamespaces)
            .Concat(PresentationNamespaces)
            .Concat(ContractNamespaces)
            .Append("FiapCloudGames.Application.Common")
            .Append("Microsoft.AspNetCore")
            .Append("Microsoft.EntityFrameworkCore")
            .ToArray();

        AssertNoDependencyOnAny(
            assembly,
            forbiddenDependencies);
    }

    [Theory]
    [MemberData(nameof(DomainAssemblies))]
    public void DomainShouldNotReferenceOtherModuleDomains(
        Assembly assembly)
    {
        var ownDomain = assembly.GetName().Name;

        AssertNoDependencyOnAny(
            assembly,
            DomainNamespaces
                .Where(item => item != ownDomain)
                .ToArray());
    }

    [Theory]
    [MemberData(nameof(ApplicationAssemblies))]
    public void ApplicationShouldNotReferenceOuterLayers(
        Assembly assembly)
    {
        AssertNoDependencyOnAny(
            assembly,
            InfrastructureNamespaces
                .Concat(PresentationNamespaces)
                .ToArray());
    }

    [Theory]
    [MemberData(nameof(ApplicationAssemblies))]
    public void ApplicationShouldNotReferenceOtherModuleImplementations(
        Assembly assembly)
    {
        var ownApplication = assembly.GetName().Name!;
        var ownDomain = ownApplication.Replace(
            ".Application",
            ".Domain",
            StringComparison.Ordinal);

        AssertNoDependencyOnAny(
            assembly,
            ApplicationNamespaces
                .Where(item => item != ownApplication)
                .Concat(DomainNamespaces.Where(item => item != ownDomain))
                .ToArray());
    }

    [Theory]
    [MemberData(nameof(InfrastructureAssemblies))]
    public void InfrastructureShouldNotReferencePresentation(
        Assembly assembly)
    {
        AssertNoDependencyOnAny(
            assembly,
            PresentationNamespaces);
    }

    [Theory]
    [MemberData(nameof(InfrastructureAssemblies))]
    public void InfrastructureShouldStayWithinItsModule(
        Assembly assembly)
    {
        var ownInfrastructure = assembly.GetName().Name!;
        var modulePrefix = ownInfrastructure[..
            ^".Infrastructure".Length];

        AssertNoDependencyOnAny(
            assembly,
            ApplicationNamespaces
                .Where(item => item != $"{modulePrefix}.Application")
                .Concat(DomainNamespaces.Where(
                    item => item != $"{modulePrefix}.Domain"))
                .Concat(InfrastructureNamespaces.Where(
                    item => item != ownInfrastructure))
                .ToArray());
    }

    [Theory]
    [MemberData(nameof(ApplicationAssemblies))]
    public void ApplicationServicesShouldNotReturnModuleContracts(
        Assembly assembly)
    {
        var invalidReturns = assembly.GetTypes()
            .Where(type =>
                type.IsPublic &&
                type.Name.EndsWith(
                    "Service",
                    StringComparison.Ordinal))
            .SelectMany(type => type.GetMethods(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly))
            .SelectMany(method => ExpandType(method.ReturnType))
            .Where(type =>
                type.Namespace?.Contains(
                    ".Contracts",
                    StringComparison.Ordinal) == true)
            .Select(type => type.FullName)
            .Distinct()
            .ToArray();

        Assert.Empty(invalidReturns);
    }

    [Theory]
    [MemberData(nameof(PresentationAssemblies))]
    public void PresentationShouldNotExposeModuleContracts(
        Assembly assembly)
    {
        AssertNoDependencyOnAny(
            assembly,
            ContractNamespaces);
    }

    [Theory]
    [MemberData(nameof(PresentationAssemblies))]
    public void PresentationShouldNotReferenceDomainOrInfrastructure(
        Assembly assembly)
    {
        AssertNoDependencyOnAny(
            assembly,
            DomainNamespaces
                .Concat(InfrastructureNamespaces)
                .ToArray());
    }

    [Fact]
    public void PresentationCommonShouldRemainLayerIndependent()
    {
        var assembly = typeof(
            Presentation.Common.PresentationCommonAssemblyReference)
            .Assembly;

        AssertNoDependencyOnAny(
            assembly,
            DomainNamespaces
                .Concat(ApplicationNamespaces)
                .Concat(InfrastructureNamespaces)
                .Concat(ContractNamespaces)
                .Append("FiapCloudGames.Domain.Common")
                .Append("FiapCloudGames.Application.Common")
                .ToArray());
    }

    [Theory]
    [MemberData(nameof(PresentationAssemblies))]
    public void ControllerActionsShouldReturnPresentationResponses(
        Assembly assembly)
    {
        var invalidReturns = assembly.GetTypes()
            .Where(type =>
                type.IsPublic &&
                type.Name.EndsWith(
                    "Controller",
                    StringComparison.Ordinal))
            .SelectMany(type => type.GetMethods(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly))
            .SelectMany(method => ExpandType(method.ReturnType))
            .Where(type =>
                type.Namespace?.Contains(
                    ".Application",
                    StringComparison.Ordinal) == true ||
                type.Namespace?.Contains(
                    ".Contracts",
                    StringComparison.Ordinal) == true)
            .Select(type => type.FullName)
            .Distinct()
            .ToArray();

        Assert.Empty(invalidReturns);
    }

    [Theory]
    [MemberData(nameof(ContractAssemblies))]
    public void ContractsShouldRemainImplementationIndependent(
        Assembly assembly)
    {
        AssertNoDependencyOnAny(
            assembly,
            DomainNamespaces
                .Concat(ApplicationNamespaces)
                .Concat(InfrastructureNamespaces)
                .Concat(PresentationNamespaces)
                .Append("Microsoft.EntityFrameworkCore")
                .Append("Microsoft.AspNetCore")
                .ToArray());
    }

    [Theory]
    [MemberData(nameof(ContractAssemblies))]
    public void ContractTypesShouldExpressTheirBoundaryRole(
        Assembly assembly)
    {
        var invalidTypes = assembly.GetExportedTypes()
            .Where(type =>
                !(type.IsInterface &&
                  type.Name.EndsWith(
                      "Module",
                      StringComparison.Ordinal)) &&
                !type.Name.EndsWith(
                    "Query",
                    StringComparison.Ordinal) &&
                !type.Name.EndsWith(
                    "Snapshot",
                    StringComparison.Ordinal))
            .Select(type => type.FullName)
            .ToArray();

        Assert.Empty(invalidTypes);
    }

    [Fact]
    public void DomainEntitiesShouldNotExposePublicSetters()
    {
        var entityTypes = DomainAssemblies()
            .Select(data => (Assembly)data[0])
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                type.Namespace?.Contains(
                    ".Domain.Entities",
                    StringComparison.Ordinal) == true);

        var publicSetters = entityTypes
            .SelectMany(type => type.GetProperties())
            .Where(property => property.SetMethod?.IsPublic == true)
            .Select(property =>
                $"{property.DeclaringType?.FullName}.{property.Name}")
            .ToArray();

        Assert.Empty(publicSetters);
    }

    [Fact]
    public void AggregateRootsShouldNotExposePublicConstructors()
    {
        Type[] aggregateRoots =
        [
            typeof(User),
            typeof(Game),
            typeof(GameLibrary),
            typeof(Promotion)
        ];

        var publicConstructors = aggregateRoots
            .Where(type => type.GetConstructors().Length > 0)
            .Select(type => type.FullName)
            .ToArray();

        Assert.Empty(publicConstructors);
    }

    [Fact]
    public void MigrationsShouldNotReferenceModulePresentation()
    {
        AssertNoDependencyOnAny(
            typeof(MigrationDbContextOptions).Assembly,
            PresentationNamespaces);
    }

    private static void AssertNoDependencyOnAny(
        Assembly assembly,
        params string[] dependencies)
    {
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(dependencies)
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            string.Join(
                Environment.NewLine,
                result.FailingTypeNames ?? []));
    }

    private static IEnumerable<Type> ExpandType(Type type)
    {
        yield return type;

        if (!type.IsGenericType)
        {
            yield break;
        }

        foreach (var argument in type.GetGenericArguments())
        {
            foreach (var nestedType in ExpandType(argument))
            {
                yield return nestedType;
            }
        }
    }
}
