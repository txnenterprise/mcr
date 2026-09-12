using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCR.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSatelliteScene : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SatelliteScenes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    TalhaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Collection = table.Column<string>(type: "text", nullable: false),
                    SceneId = table.Column<string>(type: "text", nullable: false),
                    AcquisitionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CloudCover = table.Column<decimal>(type: "numeric", nullable: true),
                    AssetUrl = table.Column<string>(type: "text", nullable: false),
                    Bbox = table.Column<string>(type: "text", nullable: false),
                    AssetsJson = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SatelliteScenes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SatelliteScenes");
        }
    }
}
