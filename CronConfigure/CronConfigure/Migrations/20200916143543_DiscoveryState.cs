using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CronConfigure.Migrations
{
    public partial class DiscoveryState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessDiscoverStateJob",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    JobId = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    DateJob = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessDiscoverStateJob", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessDiscoverStateJob_JobId",
                table: "ProcessDiscoverStateJob",
                column: "JobId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessDiscoverStateJob");
        }
    }
}
