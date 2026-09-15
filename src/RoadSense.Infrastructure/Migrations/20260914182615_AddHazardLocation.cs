using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace RoadSense.Infrastructure.Migrations
{
    public partial class AddHazardLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "Hazards",
                type: "geography (point, 4326)",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Hazards"
                SET "Location" = ST_SetSRID(
                    ST_MakePoint("Longitude", "Latitude"),
                    4326
                )::geography;
                """);

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "Hazards",
                type: "geography (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography (point, 4326)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Hazards");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }
    }
}