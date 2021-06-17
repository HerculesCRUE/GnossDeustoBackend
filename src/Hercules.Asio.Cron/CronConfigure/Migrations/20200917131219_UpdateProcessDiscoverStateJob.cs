using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CronConfigure.Migrations
{
    /// <summary>
    /// UpdateProcessDiscoverStateJob.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class UpdateProcessDiscoverStateJob : Migration
    {
        /// <summary>
        /// Up.
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateJob",
                table: "ProcessDiscoverStateJob");
        }

        /// <summary>
        /// Down.
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateJob",
                table: "ProcessDiscoverStateJob",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
