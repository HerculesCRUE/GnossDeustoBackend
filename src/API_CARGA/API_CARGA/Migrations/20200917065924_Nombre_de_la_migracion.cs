using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace API_CARGA.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class Nombre_de_la_migracion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobCreatedDate",
                table: "DiscoverItem");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobCreatedDate",
                table: "DiscoverItem",
                type: "text",
                nullable: true);
        }
    }
}
