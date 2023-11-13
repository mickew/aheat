using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AHeat.Web.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class addPowerDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PowerDevices",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceType = table.Column<int>(type: "INTEGER", nullable: false),
                    HostName = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceName = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceId = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceMac = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceModel = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceGen = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerDevices", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PowerDevices");
        }
    }
}
