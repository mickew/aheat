using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AHeat.Web.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClimateDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClimateDevices",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClimateDevices", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ClimateDeviceReadings",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClimateDeviceID = table.Column<int>(type: "INTEGER", nullable: false),
                    Temperature = table.Column<double>(type: "REAL", nullable: true),
                    Humidity = table.Column<double>(type: "REAL", nullable: true),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClimateDeviceReadings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ClimateDeviceReadings_ClimateDevices_ClimateDeviceID",
                        column: x => x.ClimateDeviceID,
                        principalTable: "ClimateDevices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClimateDeviceReadings_ClimateDeviceID",
                table: "ClimateDeviceReadings",
                column: "ClimateDeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_ClimateDevices_DeviceId",
                table: "ClimateDevices",
                column: "DeviceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClimateDeviceReadings");

            migrationBuilder.DropTable(
                name: "ClimateDevices");
        }
    }
}
