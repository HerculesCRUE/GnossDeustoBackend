using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API_CARGA.Migrations
{
    public partial class add_RepositoryID_to_ShapeConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityClass",
                table: "ShapeConfig");

            migrationBuilder.AddColumn<Guid>(
                name: "RepositoryID",
                table: "ShapeConfig",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepositoryID",
                table: "ShapeConfig");

            migrationBuilder.AddColumn<string>(
                name: "EntityClass",
                table: "ShapeConfig",
                type: "text",
                nullable: true);
        }
    }
}
