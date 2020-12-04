using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class ChampDevRanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgeDevRanges",
                table: "Championships",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SkillDevRanges",
                table: "Championships",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgeDevRanges",
                table: "Championships");

            migrationBuilder.DropColumn(
                name: "SkillDevRanges",
                table: "Championships");
        }
    }
}
