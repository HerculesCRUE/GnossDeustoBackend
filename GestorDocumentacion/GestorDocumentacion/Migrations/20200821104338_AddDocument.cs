using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GestorDocumentacion.Migrations
{
    public partial class AddDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    DocumentId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    SavedRoute = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.DocumentId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Document_Name",
                table: "Document",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Template",
                table: "Template");

            migrationBuilder.DropColumn(
                name: "TemplateID",
                table: "Template");

            migrationBuilder.AddColumn<Guid>(
                name: "PageID",
                table: "Template",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Template",
                table: "Template",
                column: "PageID");
        }
    }
}
