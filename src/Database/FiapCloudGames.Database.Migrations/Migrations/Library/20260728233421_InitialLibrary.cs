using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiapCloudGames.Database.Migrations.Migrations.Library
{
    /// <inheritdoc />
    public partial class InitialLibrary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "library");

            migrationBuilder.CreateTable(
                name: "game_libraries",
                schema: "library",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_libraries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "library_games",
                schema: "library",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    library_id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price_paid = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    promotion_id = table.Column<Guid>(type: "uuid", nullable: true),
                    acquired_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_library_games", x => x.id);
                    table.ForeignKey(
                        name: "FK_library_games_game_libraries_library_id",
                        column: x => x.library_id,
                        principalSchema: "library",
                        principalTable: "game_libraries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_game_libraries_user_id",
                schema: "library",
                table: "game_libraries",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_library_games_game_id",
                schema: "library",
                table: "library_games",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "IX_library_games_library_id_game_id",
                schema: "library",
                table: "library_games",
                columns: new[] { "library_id", "game_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "library_games",
                schema: "library");

            migrationBuilder.DropTable(
                name: "game_libraries",
                schema: "library");
        }
    }
}
