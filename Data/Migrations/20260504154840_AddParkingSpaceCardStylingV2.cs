using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarparkManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddParkingSpaceCardStylingV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardBackgroundColor",
                table: "LayoutPositions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardFontWeight",
                table: "LayoutPositions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardTextColor",
                table: "LayoutPositions",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardBackgroundColor",
                table: "LayoutPositions");

            migrationBuilder.DropColumn(
                name: "CardFontWeight",
                table: "LayoutPositions");

            migrationBuilder.DropColumn(
                name: "CardTextColor",
                table: "LayoutPositions");
        }
    }
}
