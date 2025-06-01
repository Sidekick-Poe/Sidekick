using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sidekick.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStashTabSelectedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Selected",
                table: "WealthStashes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Selected",
                table: "WealthStashes");
        }
    }
}
