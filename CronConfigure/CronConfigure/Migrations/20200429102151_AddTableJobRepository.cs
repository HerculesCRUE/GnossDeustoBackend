using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CronConfigure.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddTableJobRepository : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobRepository",
                schema: "hangfire",
                columns: table => new
                {
                    IdJob = table.Column<string>(nullable: false),
                    IdRepository = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobRepository", x => x.IdJob);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.DropTable(
                name: "JobRepository",
                schema: "hangfire");
        }
    }
}
