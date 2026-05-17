using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarparkManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOverrideRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OverriddenBookingId",
                table: "Bookings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OverriddenRequesterName",
                table: "Bookings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_OverriddenBookingId",
                table: "Bookings",
                column: "OverriddenBookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Bookings_OverriddenBookingId",
                table: "Bookings",
                column: "OverriddenBookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Bookings_OverriddenBookingId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_OverriddenBookingId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "OverriddenBookingId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "OverriddenRequesterName",
                table: "Bookings");
        }
    }
}
