using FluentMigrator;

namespace FiapCloudGames.Database.Migrations.Migrations;

[Migration(202607220001)]
public sealed class InitialDatabase : Migration
{
    public override void Up()
    {
        Create.Schema("identity");
        Create.Schema("catalog");
        Create.Schema("library");
        Create.Schema("promotions");

        Create.Table("users").InSchema("identity")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(120).NotNullable()
            .WithColumn("email").AsString(254).NotNullable()
            .WithColumn("password_hash").AsString(500).NotNullable()
            .WithColumn("role").AsInt32().NotNullable().WithDefaultValue(1)
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("created_at_utc").AsDateTimeOffset().NotNullable();
        Create.Index("ux_identity_users_email").OnTable("users").InSchema("identity")
            .OnColumn("email").Ascending().WithOptions().Unique();

        Create.Table("games").InSchema("catalog")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("title").AsString(160).NotNullable()
            .WithColumn("description").AsString(4000).NotNullable()
            .WithColumn("category").AsString(80).NotNullable()
            .WithColumn("base_price").AsDecimal(12, 2).NotNullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("created_at_utc").AsDateTimeOffset().NotNullable();
        Create.Index("ix_catalog_games_title").OnTable("games").InSchema("catalog")
            .OnColumn("title").Ascending();

        Create.Table("promotions").InSchema("promotions")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(120).NotNullable()
            .WithColumn("discount_percent").AsDecimal(5, 2).NotNullable()
            .WithColumn("starts_at_utc").AsDateTimeOffset().NotNullable()
            .WithColumn("ends_at_utc").AsDateTimeOffset().NotNullable()
            .WithColumn("ended_at_utc").AsDateTimeOffset().Nullable()
            .WithColumn("created_at_utc").AsDateTimeOffset().NotNullable();
        Create.Index("ix_promotions_period").OnTable("promotions").InSchema("promotions")
            .OnColumn("starts_at_utc").Ascending()
            .OnColumn("ends_at_utc").Ascending();

        Create.Table("promotion_games").InSchema("promotions")
            .WithColumn("promotion_id").AsGuid().NotNullable()
                .ForeignKey("fk_promotion_games_promotion", "promotions", "promotions", "id")
                .OnDelete(System.Data.Rule.Cascade)
            .WithColumn("game_id").AsGuid().NotNullable();
        Create.PrimaryKey("pk_promotion_games")
            .OnTable("promotion_games").WithSchema("promotions")
            .Columns("promotion_id", "game_id");
        Create.Index("ix_promotion_games_game_id").OnTable("promotion_games").InSchema("promotions")
            .OnColumn("game_id").Ascending();

        Create.Table("game_libraries").InSchema("library")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("user_id").AsGuid().NotNullable()
            .WithColumn("created_at_utc").AsDateTimeOffset().NotNullable();
        Create.Index("ux_game_libraries_user_id").OnTable("game_libraries").InSchema("library")
            .OnColumn("user_id").Ascending().WithOptions().Unique();

        Create.Table("library_games").InSchema("library")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("library_id").AsGuid().NotNullable()
                .ForeignKey("fk_library_games_library", "library", "game_libraries", "id")
                .OnDelete(System.Data.Rule.Cascade)
            .WithColumn("game_id").AsGuid().NotNullable()
            .WithColumn("price_paid").AsDecimal(12, 2).NotNullable()
            .WithColumn("promotion_id").AsGuid().Nullable()
            .WithColumn("acquired_at_utc").AsDateTimeOffset().NotNullable();
        Create.Index("ux_library_games_library_game").OnTable("library_games").InSchema("library")
            .OnColumn("library_id").Ascending()
            .OnColumn("game_id").Ascending()
            .WithOptions().Unique();
        Create.Index("ix_library_games_game_id").OnTable("library_games").InSchema("library")
            .OnColumn("game_id").Ascending();

        Execute.Sql("ALTER TABLE catalog.games ADD CONSTRAINT ck_games_base_price CHECK (base_price >= 0);");
        Execute.Sql("ALTER TABLE promotions.promotions ADD CONSTRAINT ck_promotions_discount CHECK (discount_percent > 0 AND discount_percent <= 100);");
        Execute.Sql("ALTER TABLE promotions.promotions ADD CONSTRAINT ck_promotions_period CHECK (ends_at_utc > starts_at_utc);");
        Execute.Sql("ALTER TABLE library.library_games ADD CONSTRAINT ck_library_games_price CHECK (price_paid >= 0);");
    }

    public override void Down()
    {
        Delete.Schema("library");
        Delete.Schema("promotions");
        Delete.Schema("catalog");
        Delete.Schema("identity");
    }
}
