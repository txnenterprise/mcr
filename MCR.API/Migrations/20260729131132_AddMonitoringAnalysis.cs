using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCR.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMonitoringAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonitoringAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    TalhaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    SatelliteSceneId = table.Column<Guid>(type: "uuid", nullable: false),
                    SceneId = table.Column<string>(type: "text", nullable: false),
                    AcquisitionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AverageNdvi = table.Column<decimal>(type: "numeric", nullable: false),
                    MinimumNdvi = table.Column<decimal>(type: "numeric", nullable: false),
                    MaximumNdvi = table.Column<decimal>(type: "numeric", nullable: false),
                    ValidPixelCount = table.Column<int>(type: "integer", nullable: false),
                    HealthyAreaPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    CriticalAreaPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    Classification = table.Column<string>(type: "text", nullable: false),
                    PreviewImageBase64 = table.Column<string>(type: "text", nullable: false),
                    JsonStatistics = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringAnalyses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonitoringAnalyses");
        }
    }
}
