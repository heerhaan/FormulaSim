using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class currenttyreofdriverres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TyreLife",
                table: "StintResults");

            migrationBuilder.AddColumn<int>(
                name: "CurrTyreId",
                table: "DriverResults",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TyreLife",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DriverResults_CurrTyreId",
                table: "DriverResults",
                column: "CurrTyreId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverResults_Tyres_CurrTyreId",
                table: "DriverResults",
                column: "CurrTyreId",
                principalTable: "Tyres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverResults_Tyres_CurrTyreId",
                table: "DriverResults");

            migrationBuilder.DropIndex(
                name: "IX_DriverResults_CurrTyreId",
                table: "DriverResults");

            migrationBuilder.DropColumn(
                name: "CurrTyreId",
                table: "DriverResults");

            migrationBuilder.DropColumn(
                name: "TyreLife",
                table: "DriverResults");

            migrationBuilder.AddColumn<int>(
                name: "TyreLife",
                table: "StintResults",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
