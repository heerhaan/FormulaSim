using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class QualyColours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Accent",
                table: "Qualification",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Colour",
                table: "Qualification",
                maxLength: 7,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accent",
                table: "Qualification");

            migrationBuilder.DropColumn(
                name: "Colour",
                table: "Qualification");
        }
    }
}