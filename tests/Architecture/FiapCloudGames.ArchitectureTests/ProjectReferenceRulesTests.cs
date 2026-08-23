using System.Xml.Linq;

namespace FiapCloudGames.ArchitectureTests;

public sealed class ProjectReferenceRulesTests
{
    private const string Api = "FiapCloudGames.Api";
    private const string ApplicationCommon = "FiapCloudGames.Application.Common";
    private const string DomainCommon = "FiapCloudGames.Domain.Common";
    private const string PresentationCommon = "FiapCloudGames.Presentation.Common";
    private const string Migrations = "FiapCloudGames.Database.Migrations";

    [Fact]
    public void ProductionProjectReferencesShouldFollowDependencyRule()
    {
        var projectFiles = FindProjectFiles(Path.Combine(FindSolutionRoot(), "src"));
        var violations = projectFiles
            .SelectMany(GetProjectReferences)
            .Where(reference => !IsAllowed(reference.Source, reference.Target))
            .Select(reference => $"{reference.Source} -> {reference.Target}")
            .Order(StringComparer.Ordinal)
            .ToArray();

        AssertNoViolations("Referências de projeto inválidas", violations);
    }

    [Fact]
    public void UnitTests_ShouldNotReferenceInfrastructure()
    {
        AssertUnitTestsDoNotReferenceProjects(
            "Infrastructure",
            target => GetLayer(target) == "Infrastructure");
    }

    [Fact]
    public void UnitTests_ShouldNotReferencePresentation()
    {
        AssertUnitTestsDoNotReferenceProjects(
            "Presentation",
            target =>
                GetLayer(target) == "Presentation" ||
                target == PresentationCommon);
    }

    [Fact]
    public void UnitTests_ShouldNotReferenceApi()
    {
        var projectViolations = GetUnitTestProjects()
            .SelectMany(GetProjectReferences)
            .Where(reference => reference.Target == Api)
            .Select(reference => $"{reference.Source} -> {reference.Target}");
        var packageViolations = GetUnitTestProjects()
            .SelectMany(GetPackageReferences)
            .Where(reference =>
                reference.Target == "Microsoft.AspNetCore.Mvc.Testing")
            .Select(reference => $"{reference.Source} -> {reference.Target}");
        var sourceViolations = FindForbiddenSourceUsages(
            "WebApplicationFactory");

        AssertNoViolations(
            "UnitTests acoplados à API",
            projectViolations
                .Concat(packageViolations)
                .Concat(sourceViolations)
                .Order(StringComparer.Ordinal)
                .ToArray());
    }

    [Fact]
    public void UnitTests_ShouldNotReferenceEfCore()
    {
        var packageViolations = GetUnitTestProjects()
            .SelectMany(GetPackageReferences)
            .Where(reference =>
                reference.Target.Contains(
                    "EntityFrameworkCore",
                    StringComparison.Ordinal))
            .Select(reference => $"{reference.Source} -> {reference.Target}");
        var namespaceViolations = FindForbiddenSourceUsages(
            "Microsoft.EntityFrameworkCore");
        var contextViolations = FindForbiddenSourceUsages("DbContext");

        AssertNoViolations(
            "UnitTests acoplados ao EF Core",
            packageViolations
                .Concat(namespaceViolations)
                .Concat(contextViolations)
                .Order(StringComparer.Ordinal)
                .ToArray());
    }

    [Fact]
    public void UnitTests_ShouldNotReferenceDatabaseMigrations()
    {
        AssertUnitTestsDoNotReferenceProjects(
            "Database.Migrations",
            target => target == Migrations);
    }

    private static void AssertUnitTestsDoNotReferenceProjects(
        string rule,
        Func<string, bool> isForbidden)
    {
        var violations = GetUnitTestProjects()
            .SelectMany(GetProjectReferences)
            .Where(reference => isForbidden(reference.Target))
            .Select(reference => $"{reference.Source} -> {reference.Target}")
            .Order(StringComparer.Ordinal)
            .ToArray();

        AssertNoViolations($"UnitTests referenciando {rule}", violations);
    }

    private static string[] FindForbiddenSourceUsages(string forbiddenText) =>
        GetUnitTestProjects()
            .SelectMany(project =>
            {
                var projectDirectory = Path.GetDirectoryName(project)!;
                return Directory
                    .GetFiles(projectDirectory, "*.cs", SearchOption.AllDirectories)
                    .Where(path => !IsBuildArtifact(path))
                    .Where(path => File.ReadAllText(path).Contains(
                        forbiddenText,
                        StringComparison.Ordinal))
                    .Select(path =>
                        $"{Path.GetFileNameWithoutExtension(project)} -> " +
                        Path.GetRelativePath(projectDirectory, path));
            })
            .ToArray();

    private static bool IsBuildArtifact(string path)
    {
        var segments = path.Split(
            [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
            StringSplitOptions.RemoveEmptyEntries);
        return segments.Contains("bin", StringComparer.OrdinalIgnoreCase) ||
               segments.Contains("obj", StringComparer.OrdinalIgnoreCase);
    }

    private static string[] GetUnitTestProjects() =>
        FindProjectFiles(Path.Combine(FindSolutionRoot(), "tests", "Unit"));

    private static string[] FindProjectFiles(string root) =>
        Directory
            .GetFiles(root, "*.csproj", SearchOption.AllDirectories)
            .Order(StringComparer.Ordinal)
            .ToArray();

    private static IEnumerable<ProjectDependency> GetProjectReferences(
        string projectFile)
    {
        var source = Path.GetFileNameWithoutExtension(projectFile);
        var projectDirectory = Path.GetDirectoryName(projectFile)!;
        var document = XDocument.Load(projectFile);

        foreach (var element in document
                     .Descendants()
                     .Where(item => item.Name.LocalName == "ProjectReference"))
        {
            var include = element.Attribute("Include")?.Value;
            Assert.False(string.IsNullOrWhiteSpace(include));

            foreach (var targetPath in ResolveInclude(projectDirectory, include!))
            {
                Assert.True(
                    File.Exists(targetPath),
                    $"Projeto referenciado não encontrado: {targetPath}");

                yield return new ProjectDependency(
                    source,
                    Path.GetFileNameWithoutExtension(targetPath));
            }
        }
    }

    private static IEnumerable<ProjectDependency> GetPackageReferences(
        string projectFile)
    {
        var source = Path.GetFileNameWithoutExtension(projectFile);
        var document = XDocument.Load(projectFile);

        return document
            .Descendants()
            .Where(item => item.Name.LocalName == "PackageReference")
            .Select(element => element.Attribute("Include")?.Value)
            .Where(include => !string.IsNullOrWhiteSpace(include))
            .Select(include => new ProjectDependency(source, include!));
    }

    private static IEnumerable<string> ResolveInclude(
        string projectDirectory,
        string include)
    {
        if (!include.Contains('*'))
        {
            yield return Path.GetFullPath(include, projectDirectory);
            yield break;
        }

        var normalized = include.Replace(
            Path.AltDirectorySeparatorChar,
            Path.DirectorySeparatorChar);
        var wildcardIndex = normalized.IndexOf('*');
        var prefix = normalized[..wildcardIndex];
        var searchRoot = Path.GetFullPath(
            prefix[..prefix.LastIndexOf(Path.DirectorySeparatorChar)],
            projectDirectory);

        foreach (var project in Directory.GetFiles(
                     searchRoot,
                     "*.csproj",
                     SearchOption.AllDirectories))
        {
            yield return project;
        }
    }

    private static bool IsAllowed(string source, string target)
    {
        if (source == Api)
        {
            return target is ApplicationCommon or DomainCommon or PresentationCommon ||
                   GetLayer(target) is "Infrastructure" or "Presentation";
        }

        if (source == Migrations)
        {
            return GetLayer(target) == "Infrastructure";
        }

        if (source is ApplicationCommon or DomainCommon or PresentationCommon)
        {
            return false;
        }

        var sourceLayer = GetLayer(source);
        var targetLayer = GetLayer(target);
        var sameModule = GetModule(source) == GetModule(target);

        return sourceLayer switch
        {
            "Contracts" => false,
            "Domain" => target == DomainCommon,
            "Application" =>
                target == ApplicationCommon ||
                targetLayer == "Contracts" ||
                sameModule && targetLayer == "Domain",
            "Infrastructure" =>
                sameModule && targetLayer is "Application" or "Domain",
            "Presentation" =>
                target == PresentationCommon ||
                sameModule && targetLayer == "Application",
            _ => false
        };
    }

    private static string GetModule(string projectName)
    {
        var segments = projectName.Split('.');
        return segments.Length > 2 ? segments[1] : string.Empty;
    }

    private static string GetLayer(string projectName) =>
        projectName.Split('.')[^1];

    private static string FindSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "FiapCloudGames.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Não foi possível localizar a raiz da solução.");
    }

    private static void AssertNoViolations(
        string description,
        IReadOnlyCollection<string> violations)
    {
        Assert.True(
            violations.Count == 0,
            description + ":" + Environment.NewLine +
            string.Join(Environment.NewLine, violations));
    }

    private sealed record ProjectDependency(string Source, string Target);
}
