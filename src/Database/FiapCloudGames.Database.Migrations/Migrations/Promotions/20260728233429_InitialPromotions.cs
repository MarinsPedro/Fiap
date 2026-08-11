using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiapCloudGames.Database.Migrations.Migrations.Promotions
{
    /// <inheritdoc />
    public partial class InitialPromotions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "promotions");

            migrationBuilder.CreateTable(
                name: "promotions",
                schema: "promotions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    discount_percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    starts_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ends_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ended_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promotions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "promotion_games",
                schema: "promotions",
                columns: table => new
                {
                    promotion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promotion_games", x => new { x.promotion_id, x.game_id });
                    table.ForeignKey(
                        name: "FK_promotion_games_promotions_promotion_id",
                        column: x => x.promotion_id,
                        principalSchema: "promotions",
                        principalTable: "promotions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_promotion_games_game_id",
                schema: "promotions",
                table: "promotion_games",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "IX_promotions_starts_at_utc_ends_at_utc",
                schema: "promotions",
                table: "promotions",
                columns: new[] { "starts_at_utc", "ends_at_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "promotion_games",
                schema: "promotions");

            migrationBuilder.DropTable(
                name: "promotions",
                schema: "promotions");
        }
    }
}
