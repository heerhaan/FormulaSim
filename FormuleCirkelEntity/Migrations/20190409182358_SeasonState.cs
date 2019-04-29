using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace FormuleCirkelEntity.Migrations
{
    public partial class SeasonState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentSeason",
                table: "Seasons");

            migrationBuilder.AddColumn<DateTime>(
                name: "SeasonStart",
                table: "Seasons",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Seasons",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeasonStart",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Seasons");

            migrationBuilder.AddColumn<bool>(
                name: "CurrentSeason",
                table: "Seasons",
                nullable: false,
                defaultValue: false);
        }
    }
}