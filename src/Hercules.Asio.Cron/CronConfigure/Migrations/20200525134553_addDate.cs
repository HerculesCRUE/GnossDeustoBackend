using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CronConfigure.Migrations
{
    /// <summary>
    /// addDate.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class addDate : Migration
    {
        /// <summary>
        /// Up.
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEjecucion",
                schema: "hangfire",
                table: "JobRepository",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <summary>
        /// Down.
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaEjecucion",
                schema: "hangfire",
                table: "JobRepository");
        }
    }
}
