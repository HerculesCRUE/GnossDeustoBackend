using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CronConfigure.Migrations
{
    /// <summary>
    /// DiscoveryState.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class DiscoveryState : Migration
    {
        /// <summary>
        /// Up.
        /// </summary>
        /// <param name="migrationBuilder"></param>
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

        /// <summary>
        /// Down.
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessDiscoverStateJob");
        }
    }
}
