using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace API_CARGA.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class DiscoverItem_LoadedEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoadedEntities",
                table: "DiscoverItem",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoadedEntities",
                table: "DiscoverItem");
        }
    }
}
