using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class SeasonSettingsFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QualificationRemainingDriversQ2",
                table: "Seasons",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QualificationRemainingDriversQ3",
                table: "Seasons",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QualificationRemainingDriversQ2",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "QualificationRemainingDriversQ3",
                table: "Seasons");
        }
    }
}
