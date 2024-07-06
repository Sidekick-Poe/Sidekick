using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sidekick.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "WealthFullSnapshots",
                columns: table => new
                {
                    Date = table.Column<long>(type: "INTEGER", nullable: false),
                    League = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Total = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WealthFullSnapshots", x => x.Date);
                });

            migrationBuilder.CreateTable(
                name: "WealthItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    StashId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    League = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    ItemLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    MapTier = table.Column<int>(type: "INTEGER", nullable: true),
                    GemLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxLinks = table.Column<int>(type: "INTEGER", nullable: true),
                    Count = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<double>(type: "REAL", nullable: false),
                    Total = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WealthItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WealthStashes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Parent = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    League = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Total = table.Column<double>(type: "REAL", nullable: false),
                    LastUpdate = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WealthStashes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WealthStashSnapshots",
                columns: table => new
                {
                    Date = table.Column<long>(type: "INTEGER", nullable: false),
                    StashId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    League = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Total = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WealthStashSnapshots", x => new { x.Date, x.StashId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "WealthFullSnapshots");

            migrationBuilder.DropTable(
                name: "WealthItems");

            migrationBuilder.DropTable(
                name: "WealthStashes");

            migrationBuilder.DropTable(
                name: "WealthStashSnapshots");
        }
    }
}
