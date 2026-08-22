using System.Xml.Linq;

namespace FiapCloudGames.ArchitectureTests;

public sealed class ProjectReferenceRulesTests
{
    private const string Api = "FiapCloudGames.Api";
    private const string ApplicationCommon =
        "FiapCloudGames.Application.Common";
    private const string DomainCommon = "FiapCloudGames.Domain.Common";
    private const string PresentationCommon =
        "FiapCloudGames.Presentation.Common";
    private const string Migrations =
        "FiapCloudGames.Database.Migrations";

    [Fact]
    public void ProductionProjectReferencesShouldFollowDependencyRule()
    {
        var sourceRoot = Path.Combine(FindSolutionRoot(), "src");
        var projectFiles = Directory.GetFiles(
            sourceRoot,
            "*.csproj",
            SearchOption.AllDirectories);

        var violations = projectFiles
            .SelectMany(GetProjectReferences)
            .Where(reference =>
                !IsAllowed(reference.Source, reference.Target))
            .Select(reference =>
                $"{reference.Source} -> {reference.Target}")
            .Order()
            .ToArray();

        Assert.True(
            violations.Length == 0,
            "Referências de projeto inválidas:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, violations));
    }

    private static IEnumerable<ProjectReference> GetProjectReferences(
        string projectFile)
    {
        var source = Path.GetFileNameWithoutExtension(projectFile);
        var projectDirectory = Path.GetDirectoryName(projectFile)!;
        var document = XDocument.Load(projectFile);

        foreach (var element in document
                     .Descendants()
                     .Where(item =>
                         item.Name.LocalName == "ProjectReference"))
        {
            var include = element.Attribute("Include")?.Value;
            Assert.False(string.IsNullOrWhiteSpace(include));

            var targetPath = Path.GetFullPath(
                include!,
                projectDirectory);
            Assert.True(
                File.Exists(targetPath),
                $"Projeto referenciado não encontrado: {targetPath}");

            yield return new ProjectReference(
                source,
                Path.GetFileNameWithoutExtension(targetPath));
        }
    }

    private static bool IsAllowed(string source, string target)
    {
        if (source == Api)
        {
            return target is ApplicationCommon or
                       DomainCommon or
                       PresentationCommon ||
                   GetLayer(target) is "Infrastructure" or "Presentation";
        }

        if (source == Migrations)
        {
            return GetLayer(target) == "Infrastructure";
        }

        if (source is ApplicationCommon or
            DomainCommon or
            PresentationCommon)
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
                sameModule &&
                targetLayer is "Application" or "Domain",
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
            if (File.Exists(
                    Path.Combine(directory.FullName, "FiapCloudGames.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Não foi possível localizar a raiz da solução.");
    }

    private sealed record ProjectReference(
        string Source,
        string Target);
}
