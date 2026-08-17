using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Database.Migrations.Configuration;

public static class MigrationDbContextOptions
{
    public const string HistorySchema = "infra";

    public const string IdentityHistoryTable =
        "__EFMigrationsHistory_Identity";

    public const string CatalogHistoryTable =
        "__EFMigrationsHistory_Catalog";

    public const string PromotionsHistoryTable =
        "__EFMigrationsHistory_Promotions";

    public const string LibraryHistoryTable =
        "__EFMigrationsHistory_Library";

    private static readonly string MigrationsAssemblyName =
        typeof(MigrationDbContextOptions).Assembly.GetName().Name!;

    public static void ConfigureIdentity(
        DbContextOptionsBuilder options,
        string connectionString) =>
        Configure(options, connectionString, IdentityHistoryTable);

    public static void ConfigureCatalog(
        DbContextOptionsBuilder options,
        string connectionString) =>
        Configure(options, connectionString, CatalogHistoryTable);

    public static void ConfigurePromotions(
        DbContextOptionsBuilder options,
        string connectionString) =>
        Configure(options, connectionString, PromotionsHistoryTable);

    public static void ConfigureLibrary(
        DbContextOptionsBuilder options,
        string connectionString) =>
        Configure(options, connectionString, LibraryHistoryTable);

    private static void Configure(
        DbContextOptionsBuilder options,
        string connectionString,
        string historyTable)
    {
        options.UseNpgsql(
            connectionString,
            npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(MigrationsAssemblyName);
                npgsqlOptions.MigrationsHistoryTable(
                    historyTable,
                    HistorySchema);
            });
    }
}
