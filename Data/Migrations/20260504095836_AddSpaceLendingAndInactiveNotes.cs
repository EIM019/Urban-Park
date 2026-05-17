using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarparkManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSpaceLendingAndInactiveNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InactiveNote",
                table: "ParkingSpaces",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InactiveSince",
                table: "ParkingSpaces",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInactivePermanent",
                table: "ParkingSpaces",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SpaceLendings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParkingSpaceId = table.Column<int>(type: "INTEGER", nullable: false),
                    OriginalType = table.Column<int>(type: "INTEGER", nullable: false),
                    LentToType = table.Column<int>(type: "INTEGER", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsReverted = table.Column<bool>(type: "INTEGER", nullable: false),
                    RevertedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RevertedByUserId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaceLendings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpaceLendings_ParkingSpaces_ParkingSpaceId",
                        column: x => x.ParkingSpaceId,
                        principalTable: "ParkingSpaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpaceLendings_ParkingSpaceId",
                table: "SpaceLendings",
                column: "ParkingSpaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpaceLendings");

            migrationBuilder.DropColumn(
                name: "InactiveNote",
                table: "ParkingSpaces");

            migrationBuilder.DropColumn(
                name: "InactiveSince",
                table: "ParkingSpaces");

            migrationBuilder.DropColumn(
                name: "IsInactivePermanent",
                table: "ParkingSpaces");
        }
    }
}
