using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadSense.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHazardLocationSpatialIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Hazards_Location",
                table: "Hazards",
                column: "Location")
                .Annotation("Npgsql:IndexMethod", "gist");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Hazards_Location",
                table: "Hazards");
        }
    }
}
