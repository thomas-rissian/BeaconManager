using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaliseManager.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Beacons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Equipment = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Site = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    BatteryLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    LastCommunicationUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beacons", x => x.Id);
                    table.CheckConstraint("CK_Beacons_BatteryLevel", "\"BatteryLevel\" IS NULL OR \"BatteryLevel\" BETWEEN 0 AND 100");
                });

            migrationBuilder.CreateTable(
                name: "BeaconEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BeaconId = table.Column<int>(type: "INTEGER", nullable: false),
                    EventType = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    BatteryLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeaconEvents", x => x.Id);
                    table.CheckConstraint("CK_BeaconEvents_BatteryLevel", "\"BatteryLevel\" IS NULL OR \"BatteryLevel\" BETWEEN 0 AND 100");
                    table.ForeignKey(
                        name: "FK_BeaconEvents_Beacons_BeaconId",
                        column: x => x.BeaconId,
                        principalTable: "Beacons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BeaconEvents_BeaconId_OccurredAtUtc",
                table: "BeaconEvents",
                columns: new[] { "BeaconId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Beacons_Code",
                table: "Beacons",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeaconEvents");

            migrationBuilder.DropTable(
                name: "Beacons");
        }
    }
}
