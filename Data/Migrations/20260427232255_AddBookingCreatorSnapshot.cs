using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarparkManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingCreatorSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "Bookings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserRole",
                table: "Bookings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CreatedByUserRole",
                table: "Bookings");
        }
    }
}
