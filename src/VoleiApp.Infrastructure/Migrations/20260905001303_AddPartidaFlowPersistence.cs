using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoleiApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPartidaFlowPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Atletas",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Posicao = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atletas", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AtletaSubstituicao",
                columns: table => new
                {
                    EntraramID = table.Column<int>(type: "INTEGER", nullable: false),
                    SubstituicaoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtletaSubstituicao", x => new { x.EntraramID, x.SubstituicaoId });
                    table.ForeignKey(
                        name: "FK_AtletaSubstituicao_Atletas_EntraramID",
                        column: x => x.EntraramID,
                        principalTable: "Atletas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AtletaSubstituicao1",
                columns: table => new
                {
                    SairamID = table.Column<int>(type: "INTEGER", nullable: false),
                    Substituicao1Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtletaSubstituicao1", x => new { x.SairamID, x.Substituicao1Id });
                    table.ForeignKey(
                        name: "FK_AtletaSubstituicao1_Atletas_SairamID",
                        column: x => x.SairamID,
                        principalTable: "Atletas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartidaReservas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartidaId = table.Column<int>(type: "INTEGER", nullable: false),
                    AtletaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ordem = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartidaReservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartidaReservas_Atletas_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "Atletas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Partidas",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimeAId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeBId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeVencedorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Encerrada = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidas", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Times",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    PartidaId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Times", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Times_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Substituicoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartidaId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Substituicoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Substituicoes_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Substituicoes_Times_TimeId",
                        column: x => x.TimeId,
                        principalTable: "Times",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimeAtletas",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimeId = table.Column<int>(type: "INTEGER", nullable: false),
                    AtletaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeAtletas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TimeAtletas_Atletas_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "Atletas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimeAtletas_Times_TimeId",
                        column: x => x.TimeId,
                        principalTable: "Times",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AtletaSubstituicao_SubstituicaoId",
                table: "AtletaSubstituicao",
                column: "SubstituicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_AtletaSubstituicao1_Substituicao1Id",
                table: "AtletaSubstituicao1",
                column: "Substituicao1Id");

            migrationBuilder.CreateIndex(
                name: "IX_PartidaReservas_AtletaId",
                table: "PartidaReservas",
                column: "AtletaId");

            migrationBuilder.CreateIndex(
                name: "IX_PartidaReservas_PartidaId",
                table: "PartidaReservas",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_TimeAId",
                table: "Partidas",
                column: "TimeAId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_TimeBId",
                table: "Partidas",
                column: "TimeBId");

            migrationBuilder.CreateIndex(
                name: "IX_Substituicoes_PartidaId",
                table: "Substituicoes",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_Substituicoes_TimeId",
                table: "Substituicoes",
                column: "TimeId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeAtletas_AtletaId",
                table: "TimeAtletas",
                column: "AtletaId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeAtletas_TimeId",
                table: "TimeAtletas",
                column: "TimeId");

            migrationBuilder.CreateIndex(
                name: "IX_Times_PartidaId",
                table: "Times",
                column: "PartidaId");

            migrationBuilder.AddForeignKey(
                name: "FK_AtletaSubstituicao_Substituicoes_SubstituicaoId",
                table: "AtletaSubstituicao",
                column: "SubstituicaoId",
                principalTable: "Substituicoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AtletaSubstituicao1_Substituicoes_Substituicao1Id",
                table: "AtletaSubstituicao1",
                column: "Substituicao1Id",
                principalTable: "Substituicoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartidaReservas_Partidas_PartidaId",
                table: "PartidaReservas",
                column: "PartidaId",
                principalTable: "Partidas",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidas_Times_TimeAId",
                table: "Partidas",
                column: "TimeAId",
                principalTable: "Times",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidas_Times_TimeBId",
                table: "Partidas",
                column: "TimeBId",
                principalTable: "Times",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Times_Partidas_PartidaId",
                table: "Times");

            migrationBuilder.DropTable(
                name: "AtletaSubstituicao");

            migrationBuilder.DropTable(
                name: "AtletaSubstituicao1");

            migrationBuilder.DropTable(
                name: "PartidaReservas");

            migrationBuilder.DropTable(
                name: "TimeAtletas");

            migrationBuilder.DropTable(
                name: "Substituicoes");

            migrationBuilder.DropTable(
                name: "Atletas");

            migrationBuilder.DropTable(
                name: "Partidas");

            migrationBuilder.DropTable(
                name: "Times");
        }
    }
}
