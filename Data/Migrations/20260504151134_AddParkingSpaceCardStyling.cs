using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarparkManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddParkingSpaceCardStyling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardBackgroundColor",
                table: "ParkingSpaces",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardFontWeight",
                table: "ParkingSpaces",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardTextColor",
                table: "ParkingSpaces",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardBackgroundColor",
                table: "ParkingSpaces");

            migrationBuilder.DropColumn(
                name: "CardFontWeight",
                table: "ParkingSpaces");

            migrationBuilder.DropColumn(
                name: "CardTextColor",
                table: "ParkingSpaces");
        }
    }
}
