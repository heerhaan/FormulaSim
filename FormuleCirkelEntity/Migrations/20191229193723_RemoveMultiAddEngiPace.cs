using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class RemoveMultiAddEngiPace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChassisMultiplier",
                table: "Traits");

            migrationBuilder.DropColumn(
                name: "DriverMultiplier",
                table: "Traits");

            migrationBuilder.DropColumn(
                name: "EngineMultiplier",
                table: "Traits");

            migrationBuilder.DropColumn(
                name: "ChassisMulti",
                table: "DriverResults");

            migrationBuilder.DropColumn(
                name: "DriverMulti",
                table: "DriverResults");

            migrationBuilder.DropColumn(
                name: "EngineMulti",
                table: "DriverResults");

            migrationBuilder.AddColumn<int>(
                name: "EngineRacePace",
                table: "Traits",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EngineRacePace",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EngineRacePace",
                table: "Traits");

            migrationBuilder.DropColumn(
                name: "EngineRacePace",
                table: "DriverResults");

            migrationBuilder.AddColumn<decimal>(
                name: "ChassisMultiplier",
                table: "Traits",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DriverMultiplier",
                table: "Traits",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EngineMultiplier",
                table: "Traits",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ChassisMulti",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DriverMulti",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EngineMulti",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
