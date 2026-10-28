using VoleiApp.Application.DTOs.Sorteio;
using VoleiApp.Application.Services;
using VoleiApp.Domain.Entities;
using VoleiApp.Domain.Enums;
using VoleiApp.Domain.Interfaces.Repositories;

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
        public async Task SortearTimes_QuandoEstoquePerfeito_DeveGerarTimesIdeaisSemIncompletos()
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
            Assert.Empty(resultado.Reservas);
            Assert.Equal(0, resultado.TimesIncompletos);
            Assert.DoesNotContain(resultado.Warnings, w => w.Contains("incompleto", StringComparison.OrdinalIgnoreCase));
        }

        // ----- Cenário 2: falta levantador -----
        [Fact]
        public async Task SortearTimes_QuandoFaltarLevantador_DeveGerarTimeIncompleto()
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

            Assert.Equal(1, resultado.TimesIncompletos);
            Assert.Equal(5, resultado.Reservas.Count);
            Assert.Contains(resultado.Warnings, w => w.Contains("incompleto", StringComparison.OrdinalIgnoreCase));
        }

        // ----- Cenário 3: falta meio/central -----
        [Fact]
        public async Task SortearTimes_QuandoFaltaMeio_DeveGerarTimeIncompleto()
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

            Assert.Equal(1, result.TimesIncompletos);
            Assert.Equal(5, result.Reservas.Count);
            Assert.Contains(result.Warnings, w => w.Contains("incompleto", StringComparison.OrdinalIgnoreCase));
        }

        // ----- Cenário 4: atletas sobrando vão para reserva -----
        [Fact]
        public async Task SortearTimes_QuandoSobramAtletas_DeveGerarTimeIncompletoComSobras()
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

            Assert.Equal(3, result.Times.Count);
            Assert.Empty(result.Reservas);
            Assert.Equal(1, result.TimesIncompletos);
            Assert.Contains(result.Warnings, w => w.Contains("incompleto", StringComparison.OrdinalIgnoreCase));
        }

        // ----- Cenário 5: lista vazia -----
        [Fact]
        public async Task SortearTimes_QuandoAListaEstiverVazia_DeveRetornarWarningSemException()
        {
            var config = new SorteioConfigDTO
            {
                TamanhoDoTime = 4,
                Atletas = AtletasIdeais(0)
            };

            var result = await _service.SortearTimes(config);

            Assert.Empty(result.Times);
            Assert.Contains(result.Warnings, w => w.Contains("não há atletas", StringComparison.OrdinalIgnoreCase));
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

        [Fact]
        public async Task SortearTimes_QuandoNaoInformarAtletas_DeveBuscarAtletasNoRepositorio()
        {
            var repository = new FakeAtletaRepository(AtletasIdeais(2));
            var service = new SorteioService(repository);

            var result = await service.SortearTimes(new SorteioConfigDTO
            {
                TamanhoDoTime = 4,
                AtacantesPorTime = 2,
                LevantadoresPorTime = 1,
                MeiosPorTime = 1
            });

            Assert.Equal(2, result.Times.Count);
            Assert.Equal(0, result.TimesIncompletos);
            Assert.Empty(result.Reservas);
        }

        [Fact]
        public async Task SortearTimes_DevePersistirTimesCriadosNoRepositorio()
        {
            var atletaRepository = new FakeAtletaRepository(AtletasIdeais(2));
            var timeRepository = new FakeTimeRepository();
            var service = new SorteioService(atletaRepository, timeRepository);

            await service.SortearTimes(new SorteioConfigDTO
            {
                TamanhoDoTime = 4,
                AtacantesPorTime = 2,
                LevantadoresPorTime = 1,
                MeiosPorTime = 1
            });

            Assert.Equal(2, timeRepository.TimesPersistidos.Count);
            Assert.All(timeRepository.TimesPersistidos, t => Assert.Equal(4, t.Atletas.Count));
        }

        // ----- Cenário 7: substituição rotaciona reserva corretamente -----
        [Fact]
        public async Task SubstituirJogadores_DeveRotacionarReservasCorretamente()
        {
            var timePerdedor = new Time
            {
                ID = 1,
                Nome = "Time 1",
                Atletas = new List<TimeAtleta>
                {
                    new TimeAtleta { Atleta = Atleta(1, "Ponteiro 1", EPosicao.Ponteiro), AtletaId = 1 },
                    new TimeAtleta { Atleta = Atleta(2, "Ponteiro 2", EPosicao.Ponteiro), AtletaId = 2 },
                    new TimeAtleta { Atleta = Atleta(3, "Meio 1", EPosicao.Central), AtletaId = 3 },
                    new TimeAtleta { Atleta = Atleta(4, "Levantador 1", EPosicao.Levantador), AtletaId = 4 },
                }
            };

            var reservas = new Queue<Atleta>();
            reservas.Enqueue(Atleta(1, "Reserva 1", EPosicao.Levantador));
            reservas.Enqueue(Atleta(2, "Reserva 2", EPosicao.Central));

            var substituicao = await _service.SubstituirJogadores(timePerdedor, reservas);

            // quem saiu não está mais no time (comparar com AtletaId dos TimeAtleta)
            Assert.DoesNotContain(timePerdedor.Atletas, ta => substituicao.Sairam.Select(s => s.ID).Contains(ta.AtletaId));

            // quem entrou está no time (comparar com AtletaId dos TimeAtleta)
            Assert.Contains(timePerdedor.Atletas, ta => substituicao.Entraram.Select(s => s.ID).Contains(ta.AtletaId));

            // quem saiu voltou para a fila de reservas
            Assert.Contains(reservas, a => substituicao.Sairam.Select(s => s.ID).Contains(a.ID));
        }

        // ----- Cenário 8: substituição sem reservas disponíveis -----
        [Fact]
        public async Task SubstituirJogadores_QuandoSemReservas_NaoDeveAlterarTime()
        {
            // Arrange
            var atletas = new List<TimeAtleta>
            {
                new TimeAtleta { Atleta = Atleta(1, "Ponteiro 1", EPosicao.Ponteiro), AtletaId = 1 },
                new TimeAtleta { Atleta = Atleta(2, "Ponteiro 2", EPosicao.Ponteiro), AtletaId = 2 },
                new TimeAtleta { Atleta = Atleta(3, "Meio 1", EPosicao.Central), AtletaId = 3 },
                new TimeAtleta { Atleta = Atleta(4, "Levantador 1", EPosicao.Levantador), AtletaId = 4 },
            };

            var timePerdedor = new Time { ID = 1, Nome = "Time 1", Atletas = atletas };
            var reservas = new Queue<Atleta>(); // vazia

            // Act
            var substituicao = await _service.SubstituirJogadores(timePerdedor, reservas);

            // Assert
            Assert.Empty(substituicao.Entraram);
            Assert.Equal(4, timePerdedor.Atletas.Count); // time intacto
        }

        private sealed class FakeAtletaRepository(List<Atleta> atletas) : IAtletaRepository
        {
            private readonly List<Atleta> _atletas = atletas;

            public Task<List<Atleta>> GetAllAsync(CancellationToken ct = default) => Task.FromResult(_atletas.ToList());

            public Task<Atleta?> GetByIdAsync(int id, CancellationToken ct = default) =>
                Task.FromResult(_atletas.FirstOrDefault(a => a.ID == id));

            public Task AddAsync(Atleta atleta, CancellationToken ct = default) => Task.CompletedTask;
            public Task UpdateAsync(Atleta atleta, CancellationToken ct = default) => Task.CompletedTask;
            public Task DeleteAsync(int id, CancellationToken ct = default) => Task.CompletedTask;
        }

        private sealed class FakeTimeRepository : ITimeRepository
        {
            public List<Time> TimesPersistidos { get; } = new();

            public Task<IEnumerable<Time>> GetAllAsync(CancellationToken ct) => Task.FromResult(TimesPersistidos.AsEnumerable());

            public Task<Time?> GetByIdAsync(int id, CancellationToken ct) =>
                Task.FromResult(TimesPersistidos.FirstOrDefault(t => t.ID == id));

            public Task AddAsync(Time time, CancellationToken ct)
            {
                TimesPersistidos.Add(time);
                return Task.CompletedTask;
            }

            public Task UpdateAsync(Time time, CancellationToken ct) => Task.CompletedTask;
            public Task DeleteAsync(Time time, CancellationToken ct) => Task.CompletedTask;
        }
    }
}
