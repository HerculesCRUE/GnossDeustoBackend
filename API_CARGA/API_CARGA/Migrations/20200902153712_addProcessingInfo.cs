using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API_CARGA.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class addProcessingInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncConfig");

            migrationBuilder.CreateTable(
                name: "ProcessingJobState",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RepositoryId = table.Column<Guid>(nullable: false),
                    JobId = table.Column<string>(nullable: true),
                    ProcessNumIdentifierOAIPMH = table.Column<int>(nullable: false),
                    TotalNumIdentifierOAIPMH = table.Column<int>(nullable: false),
                    LastIdentifierOAIPMH = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingJobState", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobState_JobId",
                table: "ProcessingJobState",
                column: "JobId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessingJobState");

            migrationBuilder.CreateTable(
                name: "SyncConfig",
                columns: table => new
                {
                    SyncConfigID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    RepositoryIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    RepositorySetIdentifiers = table.Column<string[]>(type: "text[]", nullable: true),
                    StartHour = table.Column<string>(type: "text", nullable: true),
                    UpdateFrequency = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncConfig", x => x.SyncConfigID);
                });
        }
    }
}
