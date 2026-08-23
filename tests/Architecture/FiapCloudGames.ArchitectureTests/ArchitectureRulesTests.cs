using System.Reflection;
using NetArchTest.Rules;

namespace FiapCloudGames.ArchitectureTests;

public sealed class ArchitectureRulesTests
{
    private const string ApiNamespace = "FiapCloudGames.Api";

    private static readonly string[] ApplicationNamespaces =
        ArchitectureTestAssemblies.Names(ArchitectureTestAssemblies.Applications);

    private static readonly string[] DomainNamespaces =
        ArchitectureTestAssemblies.Names(ArchitectureTestAssemblies.Domains);

    private static readonly string[] InfrastructureNamespaces =
        ArchitectureTestAssemblies.Names(ArchitectureTestAssemblies.Infrastructures);

    private static readonly string[] PresentationNamespaces =
        ArchitectureTestAssemblies
            .Names(ArchitectureTestAssemblies.Presentations)
            .Append(ArchitectureTestAssemblies.PresentationCommon.GetName().Name!)
            .ToArray();

    private static readonly string[] ContractNamespaces =
        ArchitectureTestAssemblies.Names(ArchitectureTestAssemblies.Contracts);

    public static IEnumerable<object[]> DomainAssemblies() =>
        ArchitectureTestAssemblies.AsMemberData(ArchitectureTestAssemblies.Domains);

    public static IEnumerable<object[]> ApplicationAssemblies() =>
        ArchitectureTestAssemblies.AsMemberData(ArchitectureTestAssemblies.Applications);

    public static IEnumerable<object[]> InfrastructureAssemblies() =>
        ArchitectureTestAssemblies.AsMemberData(ArchitectureTestAssemblies.Infrastructures);

    public static IEnumerable<object[]> PresentationAssemblies() =>
        ArchitectureTestAssemblies.AsMemberData(
            ArchitectureTestAssemblies.Presentations
                .Append(ArchitectureTestAssemblies.PresentationCommon));

    public static IEnumerable<object[]> ContractAssemblies() =>
        ArchitectureTestAssemblies.AsMemberData(ArchitectureTestAssemblies.Contracts);

    [Theory]
    [MemberData(nameof(DomainAssemblies))]
    public void DomainShouldBeFrameworkAndLayerIndependent(Assembly assembly)
    {
        var forbiddenDependencies = ApplicationNamespaces
            .Concat(InfrastructureNamespaces)
            .Concat(PresentationNamespaces)
            .Concat(ContractNamespaces)
            .Append(ApiNamespace)
            .Append("FiapCloudGames.Application.Common")
            .Append("Microsoft.AspNetCore")
            .Append("Microsoft.EntityFrameworkCore")
            .ToArray();

        AssertNoDependencyOnAny(assembly, forbiddenDependencies);
    }

    [Theory]
    [MemberData(nameof(DomainAssemblies))]
    public void DomainShouldNotReferenceOtherModuleDomains(Assembly assembly)
    {
        var ownDomain = assembly.GetName().Name;

        AssertNoDependencyOnAny(
            assembly,
            DomainNamespaces.Where(item => item != ownDomain).ToArray());
    }

    [Theory]
    [MemberData(nameof(ApplicationAssemblies))]
    public void ApplicationShouldNotReferenceOuterLayers(Assembly assembly)
    {
        AssertNoDependencyOnAny(
            assembly,
            InfrastructureNamespaces
                .Concat(PresentationNamespaces)
                .Append(ApiNamespace)
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
    public void InfrastructureShouldNotReferencePresentation(Assembly assembly) =>
        AssertNoDependencyOnAny(assembly, PresentationNamespaces);

    [Theory]
    [MemberData(nameof(InfrastructureAssemblies))]
    public void InfrastructureShouldStayWithinItsModule(Assembly assembly)
    {
        var ownInfrastructure = assembly.GetName().Name!;
        var modulePrefix = ownInfrastructure[..^".Infrastructure".Length];

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
        var services = assembly.GetTypes()
            .Where(type =>
                type.IsPublic &&
                type.Name.EndsWith("Service", StringComparison.Ordinal))
            .ToArray();
        Assert.NotEmpty(services);

        var invalidReturns = services
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
    public void PresentationShouldNotExposeModuleContracts(Assembly assembly) =>
        AssertNoDependencyOnAny(assembly, ContractNamespaces);

    [Theory]
    [MemberData(nameof(PresentationAssemblies))]
    public void PresentationShouldNotReferenceDomainOrInfrastructure(
        Assembly assembly)
    {
        AssertNoDependencyOnAny(
            assembly,
            DomainNamespaces.Concat(InfrastructureNamespaces).ToArray());
    }

    [Fact]
    public void PresentationCommonShouldRemainLayerIndependent()
    {
        AssertNoDependencyOnAny(
            ArchitectureTestAssemblies.PresentationCommon,
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
                type.Name.EndsWith("Controller", StringComparison.Ordinal))
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
    public void ContractsShouldRemainImplementationIndependent(Assembly assembly)
    {
        AssertNoDependencyOnAny(
            assembly,
            DomainNamespaces
                .Concat(ApplicationNamespaces)
                .Concat(InfrastructureNamespaces)
                .Concat(PresentationNamespaces)
                .Append(ApiNamespace)
                .Append("Microsoft.EntityFrameworkCore")
                .Append("Microsoft.AspNetCore")
                .ToArray());
    }

    [Theory]
    [MemberData(nameof(ContractAssemblies))]
    public void ContractTypesShouldExpressTheirBoundaryRole(Assembly assembly)
    {
        var invalidTypes = assembly.GetExportedTypes()
            .Where(type =>
                !(type.IsInterface &&
                  type.Name.EndsWith("Module", StringComparison.Ordinal)) &&
                !type.Name.EndsWith("Query", StringComparison.Ordinal) &&
                !type.Name.EndsWith("Snapshot", StringComparison.Ordinal))
            .Select(type => type.FullName)
            .ToArray();

        Assert.Empty(invalidTypes);
    }

    [Fact]
    public void DomainEntitiesShouldNotExposePublicSetters()
    {
        var entityTypes = GetDomainEntityTypes();
        Assert.NotEmpty(entityTypes);

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
        var aggregateRoots = DiscoverAggregateRoots();
        Assert.NotEmpty(aggregateRoots);

        var publicConstructors = aggregateRoots
            .Where(type => type.GetConstructors().Length > 0)
            .Select(type => type.FullName)
            .ToArray();

        Assert.Empty(publicConstructors);
    }

    [Fact]
    public void MigrationsShouldNotReferenceModulePresentation() =>
        AssertNoDependencyOnAny(
            ArchitectureTestAssemblies.Migrations,
            PresentationNamespaces);

    private static Type[] GetDomainEntityTypes() =>
        ArchitectureTestAssemblies.Domains
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                type.Namespace?.Contains(
                    ".Domain.Entities",
                    StringComparison.Ordinal) == true)
            .ToArray();

    private static Type[] DiscoverAggregateRoots()
    {
        var entityTypes = GetDomainEntityTypes().ToHashSet();

        return ArchitectureTestAssemblies.Domains
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                type.IsInterface &&
                type.Namespace?.Contains(
                    ".Domain.Repositories",
                    StringComparison.Ordinal) == true)
            .SelectMany(type => type.GetMethods())
            .SelectMany(method =>
                method.GetParameters()
                    .Select(parameter => parameter.ParameterType)
                    .Append(method.ReturnType))
            .SelectMany(ExpandType)
            .Where(entityTypes.Contains)
            .Distinct()
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToArray();
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

        if (type.HasElementType && type.GetElementType() is { } elementType)
        {
            foreach (var nestedType in ExpandType(elementType))
            {
                yield return nestedType;
            }
        }

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
