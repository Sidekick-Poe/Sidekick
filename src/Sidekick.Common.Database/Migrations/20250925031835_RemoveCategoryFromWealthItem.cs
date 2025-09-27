using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sidekick.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCategoryFromWealthItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "WealthItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "WealthItems",
                type: "TEXT",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }
    }
}
