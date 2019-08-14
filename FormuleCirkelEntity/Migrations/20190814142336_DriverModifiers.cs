using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class DriverModifiers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChassisMod",
                table: "SeasonDrivers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReliabilityMod",
                table: "SeasonDrivers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChassisMod",
                table: "SeasonDrivers");

            migrationBuilder.DropColumn(
                name: "ReliabilityMod",
                table: "SeasonDrivers");
        }
    }
}
