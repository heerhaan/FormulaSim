﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class NewSpec : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Specification",
                table: "SeasonTeams",
                newName: "Topspeed");

            migrationBuilder.AddColumn<int>(
                name: "Acceleration",
                table: "SeasonTeams",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Handling",
                table: "SeasonTeams",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Stability",
                table: "SeasonTeams",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Acceleration",
                table: "SeasonTeams");

            migrationBuilder.DropColumn(
                name: "Handling",
                table: "SeasonTeams");

            migrationBuilder.DropColumn(
                name: "Stability",
                table: "SeasonTeams");

            migrationBuilder.RenameColumn(
                name: "Topspeed",
                table: "SeasonTeams",
                newName: "Specification");
        }
    }
}