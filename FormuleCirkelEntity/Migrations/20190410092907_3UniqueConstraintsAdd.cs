using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class _3UniqueConstraintsAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Teams",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Engines",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Drivers",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Abbreviation",
                table: "Teams",
                column: "Abbreviation",
                unique: true,
                filter: "[Abbreviation] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Engines_Name",
                table: "Engines",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_Abbreviation",
                table: "Drivers",
                column: "Abbreviation",
                unique: true,
                filter: "[Abbreviation] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Teams_Abbreviation",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Engines_Name",
                table: "Engines");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_Abbreviation",
                table: "Drivers");

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Teams",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Engines",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Drivers",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}