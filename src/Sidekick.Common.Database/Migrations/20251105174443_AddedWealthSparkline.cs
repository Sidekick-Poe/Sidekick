using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sidekick.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedWealthSparkline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SparklineTotalChange",
                table: "WealthItems",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "WealthSparklines",
                columns: table => new
                {
                    ItemId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Index = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WealthSparklines", x => new { x.ItemId, x.Index });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WealthSparklines");

            migrationBuilder.DropColumn(
                name: "SparklineTotalChange",
                table: "WealthItems");
        }
    }
}
