using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class TeamsHaveColour : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Accent",
                table: "Teams",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Colour",
                table: "Teams",
                maxLength: 7,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accent",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Colour",
                table: "Teams");
        }
    }
}
