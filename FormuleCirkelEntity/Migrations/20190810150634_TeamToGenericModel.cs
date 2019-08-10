﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class TeamToGenericModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "Teams",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Teams",
                newName: "TeamId");
        }
    }
}