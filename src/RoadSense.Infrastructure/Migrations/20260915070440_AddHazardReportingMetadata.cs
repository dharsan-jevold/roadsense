using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadSense.Infrastructure.Migrations
{
    public partial class AddHazardReportingMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastReportedAtUtc",
                table: "Hazards",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportCount",
                table: "Hazards",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Hazards"
                SET
                    "ReportCount" = 1,
                    "LastReportedAtUtc" = "ReportedAtUtc";
                """);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastReportedAtUtc",
                table: "Hazards",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReportCount",
                table: "Hazards",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReportedAtUtc",
                table: "Hazards");

            migrationBuilder.DropColumn(
                name: "ReportCount",
                table: "Hazards");
        }
    }
}