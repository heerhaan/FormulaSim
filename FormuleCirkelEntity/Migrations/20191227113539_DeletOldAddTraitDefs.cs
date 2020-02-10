using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class DeletOldAddTraitDefs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DNFodds",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "RNGodds",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "ChassisMod",
                table: "SeasonDrivers");

            migrationBuilder.DropColumn(
                name: "QualyPace",
                table: "SeasonDrivers");

            migrationBuilder.DropColumn(
                name: "RacePace",
                table: "SeasonDrivers");

            migrationBuilder.DropColumn(
                name: "ReliabilityMod",
                table: "SeasonDrivers");

            migrationBuilder.RenameColumn(
                name: "Style",
                table: "SeasonDrivers",
                newName: "Reliability");

            migrationBuilder.AddColumn<decimal>(
                name: "ChassisMulti",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ChassisRacePace",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChassisRelMod",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DriverMulti",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DriverRacePace",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DriverRelMod",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "EngineMulti",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MaxRNG",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinRNG",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QualyMod",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChassisMulti",
                table: "DriverResults");

            migrationBuilder.DropColumn(
                name: "ChassisRacePace",
                table: "DriverResults");

            migrationBuilder.DropColumn(
                name: "ChassisRelMod",
                table: "DriverResults");

            migrationBuilder.DropColumn(
                name: "DriverMulti",
                table: "DriverResults");

            migrationBuilder.DropColumn(
                name: "DriverRacePace",
                table: "DriverResults");

            migrationBuilder.DropColumn(
                name: "DriverRelMod",
                table: "DriverResults");

            migrationBuilder.DropColumn(
                name: "EngineMulti",
                table: "DriverResults");

            migrationBuilder.DropColumn(
                name: "MaxRNG",
                table: "DriverResults");

            migrationBuilder.DropColumn(
                name: "MinRNG",
                table: "DriverResults");

            migrationBuilder.DropColumn(
                name: "QualyMod",
                table: "DriverResults");

            migrationBuilder.RenameColumn(
                name: "Reliability",
                table: "SeasonDrivers",
                newName: "Style");

            migrationBuilder.AddColumn<int>(
                name: "DNFodds",
                table: "Tracks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RNGodds",
                table: "Tracks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChassisMod",
                table: "SeasonDrivers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QualyPace",
                table: "SeasonDrivers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RacePace",
                table: "SeasonDrivers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReliabilityMod",
                table: "SeasonDrivers",
                nullable: false,
                defaultValue: 0);
        }
    }
}
