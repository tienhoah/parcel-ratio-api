using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace ParcelApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "Parcels",
                type: "geometry",
                nullable: true,
                computedColumnSql: "ST_SetSRID(ST_MakePoint(\"Longitude\", \"Latitude\"), 4326)",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_Location",
                table: "Parcels",
                column: "Location")
                .Annotation("Npgsql:IndexMethod", "GIST");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Parcels_Location",
                table: "Parcels");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Parcels");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }
    }
}
