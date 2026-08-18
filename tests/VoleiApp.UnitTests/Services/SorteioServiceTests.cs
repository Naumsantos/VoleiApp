using VoleiApp.Application.DTOs.Sorteio;
using VoleiApp.Application.Services;
using VoleiApp.Domain.Entities;
using VoleiApp.Domain.Enums;

namespace VoleiApp.UnitTests.Services
{
    public class SorteioServiceTests
    {
        private readonly SorteioService _service = new();

        private static Atleta Atleta(int id, string nome, EPosicao posicao) =>
            new() { ID = id, Nome = nome, Posicao = posicao };

        /// <summary>
        /// Gera uma lista com estoque exato para N times ideais (2A+1M+1L).
        /// </summary>
        private static List<Atleta> AtletasIdeais(int times) =>
        [
            .. Enumerable.Range(1, times * 2).Select(i => Atleta(i,       $"Ponteiro {i}",    EPosicao.Ponteiro)),
            .. Enumerable.Range(1, times).Select(i =>     Atleta(100 + i, $"Meio {i}",        EPosicao.Central)),
            .. Enumerable.Range(1, times).Select(i =>     Atleta(200 + i, $"Levantador {i}",  EPosicao.Levantador))
        ];

        //----- Cenário 1: times ideais -----
        [Fact]
        public async Task SortearTimes_QuandoEstoquePerfeito_DeveGerarTimesIdeaisSemFallback()
        {
            // Arrange
            var config = new SorteioConfigDTO
            {
                TamanhoDoTime = 4,
                AtacantesPorTime = 2,
                LevantadoresPorTime = 1,
                MeiosPorTime = 1,
                PermitirFallback = true,
                Atletas = AtletasIdeais(3)
            };

            // Action
            var resultado = await _service.SortearTimes(config);

            // Assert
            Assert.Equal(3, resultado.Times.Count);
            Assert.False(resultado.FallbackAplicado);
            Assert.Empty(resultado.Reservas);
            Assert.Equal(0, resultado.TimesIncompletos);
            Assert.DoesNotContain(resultado.Warnings, w => w.StartsWith("Fallback"));
        }

        // ----- Cenário 2: falta levantador -----
        [Fact]
        public async Task SortearTimes_QuandoFaltarLevantador_DeveAplicarFallbackERegistrarWarning()
        {
            // Arrange — 4 atacantes + 2 meios + SEM levantador
            var config = new SorteioConfigDTO
            {
                TamanhoDoTime = 4,
                AtacantesPorTime = 2,
                LevantadoresPorTime = 1,
                MeiosPorTime = 1,
                PermitirFallback = true,
                Atletas =
                {
                    Atleta(1, "Ponteiro 1", EPosicao.Ponteiro),
                    Atleta(2, "Ponteiro 2", EPosicao.Ponteiro),
                    Atleta(3, "Ponteiro 3", EPosicao.Ponteiro),
                    Atleta(4, "Ponteiro 4", EPosicao.Ponteiro),
                    Atleta(5, "Meio 1", EPosicao.Central),
                    Atleta(6, "Meio 2", EPosicao.Central),
                    Atleta(7, "Oposto 1", EPosicao.Oposto),
                    Atleta(8, "Oposto 2", EPosicao.Oposto),
                }
            };

            var resultado = await _service.SortearTimes(config);

            Assert.True(resultado.FallbackAplicado);
            Assert.Contains(resultado.Warnings, w => w.Contains("Fallback"));
        }

        // ----- Cenário 3: falta meio/central -----
        [Fact]
        public async Task SortearTimes_QuandoFaltaMeio_DeveAplicarFallbackERegistrarWarning()
        {
            // Arrange — 4 atacantes + 2 meios + SEM levantador
            var config = new SorteioConfigDTO
            {
                TamanhoDoTime = 4,
                AtacantesPorTime = 2,
                LevantadoresPorTime = 1,
                MeiosPorTime = 1,
                PermitirFallback = true,
                Atletas =
                {
                    Atleta(1, "Ponteiro 1", EPosicao.Ponteiro),
                    Atleta(2, "Ponteiro 2", EPosicao.Ponteiro),
                    Atleta(3, "Ponteiro 3", EPosicao.Ponteiro),
                    Atleta(4, "Ponteiro 4", EPosicao.Ponteiro),
                    Atleta(5, "Levantador 1", EPosicao.Levantador),
                    Atleta(6, "Levantador 2", EPosicao.Levantador),
                    Atleta(7, "Oposto 1", EPosicao.Oposto),
                    Atleta(8, "Oposto 2", EPosicao.Oposto),
                }
            };

            var result = await _service.SortearTimes(config);

            Assert.True(result.FallbackAplicado);
            Assert.Contains(result.Warnings, w => w.Contains("Fallback"));
        }

        // ----- Cenário 4: atletas sobrando vão para reserva -----
        [Fact]
        public async Task SortearTimes_QuandoSobramAtletas_DevePularParaReservas()
        {
            var atletas = AtletasIdeais(2);
            atletas.Add(Atleta(998, "Reserva 1", EPosicao.Ponteiro));
            atletas.Add(Atleta(999, "Reserva 2", EPosicao.Central));

            var config = new SorteioConfigDTO
            {
                TamanhoDoTime = 4,
                AtacantesPorTime = 2,
                LevantadoresPorTime = 1,
                MeiosPorTime = 1,
                PermitirFallback = true,
                Atletas = atletas
            };

            var result = await _service.SortearTimes(config);

            Assert.Equal(2, result.Times.Count);
            Assert.Equal(2, result.Reservas.Count);
            Assert.Contains(result.Warnings, w => w.Contains("reserva"));
        }

        // ----- Cenário 5: lista vazia -----
        [Fact]
        public async Task SortearTimes_QuandoAListaEstiverVazia_DeveRetornatWarningSemException()
        {
            var config = new SorteioConfigDTO
            {
                TamanhoDoTime = 4,
                Atletas = AtletasIdeais(0)
            };

            var result = await _service.SortearTimes(config);

            Assert.Empty(result.Times);
            Assert.Contains(result.Warnings, w => w.Contains("vazia"));
        }

        // ----- Cenário 6: nenhum atleta duplicado entre times -----
        [Fact]
        public async Task SortearTimes_NenhumAtletaDeveAparecerEmDoisTimes()
        {
            var config = new SorteioConfigDTO
            {
                TamanhoDoTime = 4,
                AtacantesPorTime = 2,
                LevantadoresPorTime = 1,
                MeiosPorTime = 1,
                PermitirFallback = true,
                Atletas = AtletasIdeais(4)
            };

            var result = await _service.SortearTimes(config);

            var todosIds = result.Times.SelectMany(a => a.Atletas).Select(i => i.ID).ToList();
            var idsUnicos = todosIds.Distinct().ToList();

            Assert.Equal(todosIds.Count, idsUnicos.Count);
        }
    }
}
