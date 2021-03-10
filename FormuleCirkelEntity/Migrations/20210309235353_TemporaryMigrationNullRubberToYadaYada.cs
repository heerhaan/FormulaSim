using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class TemporaryMigrationNullRubberToYadaYada : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Rubbers",
                table: "Rubbers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Rubbers");

            migrationBuilder.AddColumn<int>(
                name: "RubberId",
                table: "SeasonTeams",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RubberId",
                table: "Rubbers",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rubbers",
                table: "Rubbers",
                column: "RubberId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonTeams_RubberId",
                table: "SeasonTeams",
                column: "RubberId");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonTeams_Rubbers_RubberId",
                table: "SeasonTeams",
                column: "RubberId",
                principalTable: "Rubbers",
                principalColumn: "RubberId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonTeams_Rubbers_RubberId",
                table: "SeasonTeams");

            migrationBuilder.DropIndex(
                name: "IX_SeasonTeams_RubberId",
                table: "SeasonTeams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rubbers",
                table: "Rubbers");

            migrationBuilder.DropColumn(
                name: "RubberId",
                table: "SeasonTeams");

            migrationBuilder.DropColumn(
                name: "RubberId",
                table: "Rubbers");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Rubbers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rubbers",
                table: "Rubbers",
                column: "Id");
        }
    }
}
