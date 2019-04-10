using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class SeasonDriverList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeasonId",
                table: "SeasonDrivers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SeasonDrivers_SeasonId",
                table: "SeasonDrivers",
                column: "SeasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonDrivers_Seasons_SeasonId",
                table: "SeasonDrivers",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "SeasonId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonDrivers_Seasons_SeasonId",
                table: "SeasonDrivers");

            migrationBuilder.DropIndex(
                name: "IX_SeasonDrivers_SeasonId",
                table: "SeasonDrivers");

            migrationBuilder.DropColumn(
                name: "SeasonId",
                table: "SeasonDrivers");
        }
    }
}
