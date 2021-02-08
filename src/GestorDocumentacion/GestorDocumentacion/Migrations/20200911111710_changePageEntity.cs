using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GestorDocumentacion.Migrations
{
    public partial class changePageEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Page_Name",
                table: "Page");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Page");

            migrationBuilder.AlterColumn<string>(
                name: "Route",
                table: "Page",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Page",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRequested",
                table: "Page",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Page_Route",
                table: "Page",
                column: "Route",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Page_Route",
                table: "Page");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Page");

            migrationBuilder.DropColumn(
                name: "LastRequested",
                table: "Page");

            migrationBuilder.AlterColumn<string>(
                name: "Route",
                table: "Page",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Page",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Page_Name",
                table: "Page",
                column: "Name",
                unique: true);
        }
    }
}
