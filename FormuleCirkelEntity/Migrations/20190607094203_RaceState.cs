using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class RaceState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Drivers_Abbreviation",
                table: "Drivers");

            migrationBuilder.AddColumn<int>(
                name: "RaceState",
                table: "Races",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Drivers",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RaceState",
                table: "Races");

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Drivers",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_Abbreviation",
                table: "Drivers",
                column: "Abbreviation",
                unique: true,
                filter: "[Abbreviation] IS NOT NULL");
        }
    }
}
