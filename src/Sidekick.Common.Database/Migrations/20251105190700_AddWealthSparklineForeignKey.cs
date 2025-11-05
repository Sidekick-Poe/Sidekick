using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sidekick.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddWealthSparklineForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_WealthSparklines_WealthItems_ItemId",
                table: "WealthSparklines",
                column: "ItemId",
                principalTable: "WealthItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WealthSparklines_WealthItems_ItemId",
                table: "WealthSparklines");
        }
    }
}
