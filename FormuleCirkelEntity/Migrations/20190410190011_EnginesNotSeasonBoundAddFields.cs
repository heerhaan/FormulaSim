using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class EnginesNotSeasonBoundAddFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "Engines",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Power",
                table: "Engines",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Available",
                table: "Engines");

            migrationBuilder.DropColumn(
                name: "Power",
                table: "Engines");
        }
    }
}
