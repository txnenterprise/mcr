using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MCR.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanoCorretoraLimites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCriadorId",
                table: "PontosAtendimento",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlanoId",
                table: "Corretoras",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Planos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    MaximoUsuarios = table.Column<int>(type: "integer", nullable: true),
                    MaximoPAsPorUsuario = table.Column<int>(type: "integer", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Planos",
                columns: new[] { "Id", "Ativo", "Excluido", "MaximoPAsPorUsuario", "MaximoUsuarios", "Nome" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111101"), true, false, 2, 10, "Start" },
                    { new Guid("11111111-1111-1111-1111-111111111102"), true, false, 3, 15, "Growth" },
                    { new Guid("11111111-1111-1111-1111-111111111103"), true, false, 4, 20, "Professional" },
                    { new Guid("11111111-1111-1111-1111-111111111104"), true, false, 6, 30, "Business" },
                    { new Guid("11111111-1111-1111-1111-111111111105"), true, false, 8, 40, "Advanced" },
                    { new Guid("11111111-1111-1111-1111-111111111106"), true, false, 10, 50, "Enterprise" },
                    { new Guid("11111111-1111-1111-1111-111111111107"), true, false, null, null, "Enterprise plus" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Corretoras_PlanoId",
                table: "Corretoras",
                column: "PlanoId");

            migrationBuilder.CreateIndex(
                name: "IX_Planos_Nome",
                table: "Planos",
                column: "Nome",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Corretoras_Planos_PlanoId",
                table: "Corretoras",
                column: "PlanoId",
                principalTable: "Planos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Corretoras_Planos_PlanoId",
                table: "Corretoras");

            migrationBuilder.DropTable(
                name: "Planos");

            migrationBuilder.DropIndex(
                name: "IX_Corretoras_PlanoId",
                table: "Corretoras");

            migrationBuilder.DropColumn(
                name: "UsuarioCriadorId",
                table: "PontosAtendimento");

            migrationBuilder.DropColumn(
                name: "PlanoId",
                table: "Corretoras");
        }
    }
}
