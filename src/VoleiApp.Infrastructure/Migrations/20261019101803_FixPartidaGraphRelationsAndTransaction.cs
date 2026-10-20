using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoleiApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPartidaGraphRelationsAndTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Times_Partidas_PartidaId",
                table: "Times");

            migrationBuilder.DropTable(
                name: "AtletaSubstituicao");

            migrationBuilder.DropTable(
                name: "AtletaSubstituicao1");

            migrationBuilder.DropIndex(
                name: "IX_Times_PartidaId",
                table: "Times");

            migrationBuilder.DropIndex(
                name: "IX_TimeAtletas_TimeId",
                table: "TimeAtletas");

            migrationBuilder.DropIndex(
                name: "IX_PartidaReservas_PartidaId",
                table: "PartidaReservas");

            migrationBuilder.DropColumn(
                name: "PartidaId",
                table: "Times");

            migrationBuilder.CreateTable(
                name: "SubstituicaoEntradas",
                columns: table => new
                {
                    SubstituicaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    AtletaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubstituicaoEntradas", x => new { x.SubstituicaoId, x.AtletaId });
                    table.ForeignKey(
                        name: "FK_SubstituicaoEntradas_Atletas_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "Atletas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubstituicaoEntradas_Substituicoes_SubstituicaoId",
                        column: x => x.SubstituicaoId,
                        principalTable: "Substituicoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubstituicaoSaidas",
                columns: table => new
                {
                    SubstituicaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    AtletaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubstituicaoSaidas", x => new { x.SubstituicaoId, x.AtletaId });
                    table.ForeignKey(
                        name: "FK_SubstituicaoSaidas_Atletas_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "Atletas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubstituicaoSaidas_Substituicoes_SubstituicaoId",
                        column: x => x.SubstituicaoId,
                        principalTable: "Substituicoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeAtletas_TimeId_AtletaId",
                table: "TimeAtletas",
                columns: new[] { "TimeId", "AtletaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartidaReservas_PartidaId_AtletaId",
                table: "PartidaReservas",
                columns: new[] { "PartidaId", "AtletaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubstituicaoEntradas_AtletaId",
                table: "SubstituicaoEntradas",
                column: "AtletaId");

            migrationBuilder.CreateIndex(
                name: "IX_SubstituicaoSaidas_AtletaId",
                table: "SubstituicaoSaidas",
                column: "AtletaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubstituicaoEntradas");

            migrationBuilder.DropTable(
                name: "SubstituicaoSaidas");

            migrationBuilder.DropIndex(
                name: "IX_TimeAtletas_TimeId_AtletaId",
                table: "TimeAtletas");

            migrationBuilder.DropIndex(
                name: "IX_PartidaReservas_PartidaId_AtletaId",
                table: "PartidaReservas");

            migrationBuilder.AddColumn<int>(
                name: "PartidaId",
                table: "Times",
                type: "INTEGER",
                nullable: true);

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
                    table.ForeignKey(
                        name: "FK_AtletaSubstituicao_Substituicoes_SubstituicaoId",
                        column: x => x.SubstituicaoId,
                        principalTable: "Substituicoes",
                        principalColumn: "Id",
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
                    table.ForeignKey(
                        name: "FK_AtletaSubstituicao1_Substituicoes_Substituicao1Id",
                        column: x => x.Substituicao1Id,
                        principalTable: "Substituicoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Times_PartidaId",
                table: "Times",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeAtletas_TimeId",
                table: "TimeAtletas",
                column: "TimeId");

            migrationBuilder.CreateIndex(
                name: "IX_PartidaReservas_PartidaId",
                table: "PartidaReservas",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_AtletaSubstituicao_SubstituicaoId",
                table: "AtletaSubstituicao",
                column: "SubstituicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_AtletaSubstituicao1_Substituicao1Id",
                table: "AtletaSubstituicao1",
                column: "Substituicao1Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Times_Partidas_PartidaId",
                table: "Times",
                column: "PartidaId",
                principalTable: "Partidas",
                principalColumn: "ID");
        }
    }
}
