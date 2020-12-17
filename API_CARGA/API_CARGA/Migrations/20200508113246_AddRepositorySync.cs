using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API_CARGA.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddRepositorySync : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sincronizacion_Repositorio",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RepositoryId = table.Column<Guid>(nullable: false),
                    Set = table.Column<string>(nullable: true),
                    UltimaFechaDeSincronizacion = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sincronizacion_Repositorio", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sincronizacion_Repositorio");
        }
    }
}
