using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCR.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMonitoringTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CotacoesAgricolaProposta_CotacoesAgricola_CotacoesAgricolaId",
                table: "CotacoesAgricolaProposta");

            migrationBuilder.DropForeignKey(
                name: "FK_CulturaMaturacao_Culturas_CulturaId",
                table: "CulturaMaturacao");

            migrationBuilder.DropForeignKey(
                name: "FK_CulturaMaturacaoVariedades_CulturaMaturacao_CulturaMaturaca~",
                table: "CulturaMaturacaoVariedades");

            migrationBuilder.DropForeignKey(
                name: "FK_PropostasClientePropriedadesTalhoes_CulturaMaturacaoVarieda~",
                table: "PropostasClientePropriedadesTalhoes");

            migrationBuilder.DropForeignKey(
                name: "FK_PropostasClientePropriedadesTalhoes_CulturaMaturacao_GrupoV~",
                table: "PropostasClientePropriedadesTalhoes");

            migrationBuilder.DropForeignKey(
                name: "FK_PropostasClientePropriedadesTalhoes_PropostasClienteProprie~",
                table: "PropostasClientePropriedadesTalhoes");

            migrationBuilder.DropIndex(
                name: "IX_PropostasClientePropriedadesTalhoes_PropriedadesId",
                table: "PropostasClientePropriedadesTalhoes");

            migrationBuilder.DropIndex(
                name: "IX_CotacoesAgricolaProposta_CotacoesAgricolaId",
                table: "CotacoesAgricolaProposta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CulturaMaturacaoVariedades",
                table: "CulturaMaturacaoVariedades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CulturaMaturacao",
                table: "CulturaMaturacao");

            migrationBuilder.DropColumn(
                name: "PropriedadesId",
                table: "PropostasClientePropriedadesTalhoes");

            migrationBuilder.DropColumn(
                name: "CotacoesAgricolaId",
                table: "CotacoesAgricolaProposta");

            migrationBuilder.RenameTable(
                name: "CulturaMaturacaoVariedades",
                newName: "CulturasMaturacaoVariedades");

            migrationBuilder.RenameTable(
                name: "CulturaMaturacao",
                newName: "CulturasMaturacao");

            migrationBuilder.RenameIndex(
                name: "IX_CulturaMaturacaoVariedades_CulturaMaturacaoId",
                table: "CulturasMaturacaoVariedades",
                newName: "IX_CulturasMaturacaoVariedades_CulturaMaturacaoId");

            migrationBuilder.RenameIndex(
                name: "IX_CulturaMaturacao_CulturaId",
                table: "CulturasMaturacao",
                newName: "IX_CulturasMaturacao_CulturaId");

            migrationBuilder.AlterColumn<string>(
                name: "Observacao",
                table: "CotacoesAgricolaStatus",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "StatusAnterior",
                table: "CotacoesAgricolaStatus",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "CotacoesAgricolaStatus",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CulturasMaturacaoVariedades",
                table: "CulturasMaturacaoVariedades",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CulturasMaturacao",
                table: "CulturasMaturacao",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "MonitoringAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExecutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Severity = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Viewed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringAlerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonitoringConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    UpdateFrequency = table.Column<string>(type: "text", nullable: true),
                    CloudLimit = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonitoringExecutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Satellite = table.Column<string>(type: "text", nullable: true),
                    CloudCover = table.Column<decimal>(type: "numeric", nullable: true),
                    AverageVegetationIndex = table.Column<decimal>(type: "numeric", nullable: true),
                    AffectedArea = table.Column<decimal>(type: "numeric", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    JsonStatistics = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringExecutions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropostasClientePropriedadesTalhoes_PropostasClienteProprie~",
                table: "PropostasClientePropriedadesTalhoes",
                column: "PropostasClientePropriedadeId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesAgricolaProposta_CotacaoAgricolaId",
                table: "CotacoesAgricolaProposta",
                column: "CotacaoAgricolaId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringAlerts_ExecutionId",
                table: "MonitoringAlerts",
                column: "ExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringConfigurations_PropertyId",
                table: "MonitoringConfigurations",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringExecutions_PropertyId",
                table: "MonitoringExecutions",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CotacoesAgricolaProposta_CotacoesAgricola_CotacaoAgricolaId",
                table: "CotacoesAgricolaProposta",
                column: "CotacaoAgricolaId",
                principalTable: "CotacoesAgricola",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CulturasMaturacao_Culturas_CulturaId",
                table: "CulturasMaturacao",
                column: "CulturaId",
                principalTable: "Culturas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CulturasMaturacaoVariedades_CulturasMaturacao_CulturaMatura~",
                table: "CulturasMaturacaoVariedades",
                column: "CulturaMaturacaoId",
                principalTable: "CulturasMaturacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropostasClientePropriedadesTalhoes_CulturasMaturacaoVaried~",
                table: "PropostasClientePropriedadesTalhoes",
                column: "VariedadeId",
                principalTable: "CulturasMaturacaoVariedades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropostasClientePropriedadesTalhoes_CulturasMaturacao_Grupo~",
                table: "PropostasClientePropriedadesTalhoes",
                column: "GrupoVariedadeId",
                principalTable: "CulturasMaturacao",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropostasClientePropriedadesTalhoes_PropostasClienteProprie~",
                table: "PropostasClientePropriedadesTalhoes",
                column: "PropostasClientePropriedadeId",
                principalTable: "PropostasClientePropriedades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CotacoesAgricolaProposta_CotacoesAgricola_CotacaoAgricolaId",
                table: "CotacoesAgricolaProposta");

            migrationBuilder.DropForeignKey(
                name: "FK_CulturasMaturacao_Culturas_CulturaId",
                table: "CulturasMaturacao");

            migrationBuilder.DropForeignKey(
                name: "FK_CulturasMaturacaoVariedades_CulturasMaturacao_CulturaMatura~",
                table: "CulturasMaturacaoVariedades");

            migrationBuilder.DropForeignKey(
                name: "FK_PropostasClientePropriedadesTalhoes_CulturasMaturacaoVaried~",
                table: "PropostasClientePropriedadesTalhoes");

            migrationBuilder.DropForeignKey(
                name: "FK_PropostasClientePropriedadesTalhoes_CulturasMaturacao_Grupo~",
                table: "PropostasClientePropriedadesTalhoes");

            migrationBuilder.DropForeignKey(
                name: "FK_PropostasClientePropriedadesTalhoes_PropostasClienteProprie~",
                table: "PropostasClientePropriedadesTalhoes");

            migrationBuilder.DropTable(
                name: "MonitoringAlerts");

            migrationBuilder.DropTable(
                name: "MonitoringConfigurations");

            migrationBuilder.DropTable(
                name: "MonitoringExecutions");

            migrationBuilder.DropIndex(
                name: "IX_PropostasClientePropriedadesTalhoes_PropostasClienteProprie~",
                table: "PropostasClientePropriedadesTalhoes");

            migrationBuilder.DropIndex(
                name: "IX_CotacoesAgricolaProposta_CotacaoAgricolaId",
                table: "CotacoesAgricolaProposta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CulturasMaturacaoVariedades",
                table: "CulturasMaturacaoVariedades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CulturasMaturacao",
                table: "CulturasMaturacao");

            migrationBuilder.DropColumn(
                name: "StatusAnterior",
                table: "CotacoesAgricolaStatus");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "CotacoesAgricolaStatus");

            migrationBuilder.RenameTable(
                name: "CulturasMaturacaoVariedades",
                newName: "CulturaMaturacaoVariedades");

            migrationBuilder.RenameTable(
                name: "CulturasMaturacao",
                newName: "CulturaMaturacao");

            migrationBuilder.RenameIndex(
                name: "IX_CulturasMaturacaoVariedades_CulturaMaturacaoId",
                table: "CulturaMaturacaoVariedades",
                newName: "IX_CulturaMaturacaoVariedades_CulturaMaturacaoId");

            migrationBuilder.RenameIndex(
                name: "IX_CulturasMaturacao_CulturaId",
                table: "CulturaMaturacao",
                newName: "IX_CulturaMaturacao_CulturaId");

            migrationBuilder.AddColumn<Guid>(
                name: "PropriedadesId",
                table: "PropostasClientePropriedadesTalhoes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Observacao",
                table: "CotacoesAgricolaStatus",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CotacoesAgricolaId",
                table: "CotacoesAgricolaProposta",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CulturaMaturacaoVariedades",
                table: "CulturaMaturacaoVariedades",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CulturaMaturacao",
                table: "CulturaMaturacao",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasClientePropriedadesTalhoes_PropriedadesId",
                table: "PropostasClientePropriedadesTalhoes",
                column: "PropriedadesId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesAgricolaProposta_CotacoesAgricolaId",
                table: "CotacoesAgricolaProposta",
                column: "CotacoesAgricolaId");

            migrationBuilder.AddForeignKey(
                name: "FK_CotacoesAgricolaProposta_CotacoesAgricola_CotacoesAgricolaId",
                table: "CotacoesAgricolaProposta",
                column: "CotacoesAgricolaId",
                principalTable: "CotacoesAgricola",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CulturaMaturacao_Culturas_CulturaId",
                table: "CulturaMaturacao",
                column: "CulturaId",
                principalTable: "Culturas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CulturaMaturacaoVariedades_CulturaMaturacao_CulturaMaturaca~",
                table: "CulturaMaturacaoVariedades",
                column: "CulturaMaturacaoId",
                principalTable: "CulturaMaturacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropostasClientePropriedadesTalhoes_CulturaMaturacaoVarieda~",
                table: "PropostasClientePropriedadesTalhoes",
                column: "VariedadeId",
                principalTable: "CulturaMaturacaoVariedades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropostasClientePropriedadesTalhoes_CulturaMaturacao_GrupoV~",
                table: "PropostasClientePropriedadesTalhoes",
                column: "GrupoVariedadeId",
                principalTable: "CulturaMaturacao",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropostasClientePropriedadesTalhoes_PropostasClienteProprie~",
                table: "PropostasClientePropriedadesTalhoes",
                column: "PropriedadesId",
                principalTable: "PropostasClientePropriedades",
                principalColumn: "Id");
        }
    }
}
