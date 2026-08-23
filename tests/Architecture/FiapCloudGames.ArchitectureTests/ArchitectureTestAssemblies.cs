using System.Reflection;

namespace FiapCloudGames.ArchitectureTests;

internal static class ArchitectureTestAssemblies
{
    private const string AssemblyPrefix = "FiapCloudGames.";

    public static IReadOnlyList<Assembly> Domains { get; } = LoadLayer("Domain");

    public static IReadOnlyList<Assembly> Applications { get; } = LoadLayer("Application");

    public static IReadOnlyList<Assembly> Infrastructures { get; } = LoadLayer("Infrastructure");

    public static IReadOnlyList<Assembly> Presentations { get; } = LoadLayer("Presentation");

    public static IReadOnlyList<Assembly> Contracts { get; } = LoadLayer("Contracts");

    public static Assembly PresentationCommon { get; } = LoadExact(
        "FiapCloudGames.Presentation.Common");

    public static Assembly Migrations { get; } = LoadExact(
        "FiapCloudGames.Database.Migrations");

    public static IEnumerable<object[]> AsMemberData(
        IEnumerable<Assembly> assemblies) =>
        assemblies.Select(assembly => new object[] { assembly });

    public static string[] Names(IEnumerable<Assembly> assemblies) =>
        assemblies
            .Select(assembly => assembly.GetName().Name!)
            .Order(StringComparer.Ordinal)
            .ToArray();

    private static Assembly[] LoadLayer(string layer)
    {
        var suffix = $".{layer}";
        var assemblies = Directory
            .EnumerateFiles(
                AppContext.BaseDirectory,
                $"{AssemblyPrefix}*.dll",
                SearchOption.TopDirectoryOnly)
            .Select(path => new
            {
                Path = path,
                Name = Path.GetFileNameWithoutExtension(path)
            })
            .Where(item =>
                item.Name.StartsWith(
                    AssemblyPrefix,
                    StringComparison.Ordinal) &&
                item.Name.EndsWith(suffix, StringComparison.Ordinal) &&
                item.Name.Split('.').Length == 3)
            .OrderBy(item => item.Name, StringComparer.Ordinal)
            .Select(item => Assembly.LoadFrom(item.Path))
            .ToArray();

        if (assemblies.Length == 0)
        {
            throw new InvalidOperationException(
                $"Nenhuma assembly da camada {layer} foi descoberta.");
        }

        return assemblies;
    }

    private static Assembly LoadExact(string assemblyName)
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            $"{assemblyName}.dll");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"A assembly {assemblyName} não foi encontrada.",
                path);
        }

        return Assembly.LoadFrom(path);
    }
}
