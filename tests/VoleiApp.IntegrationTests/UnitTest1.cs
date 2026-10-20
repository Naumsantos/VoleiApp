using Microsoft.EntityFrameworkCore;
using VoleiApp.Application.DTOs.Partida;
using VoleiApp.Application.Services;
using VoleiApp.Domain.Entities;
using VoleiApp.Domain.Enums;
using VoleiApp.Infrastructure.Persistence;
using VoleiApp.Infrastructure.Repositories;

namespace VoleiApp.IntegrationTests;

public class UnitTest1
{
    [Fact]
    public async Task SalvarPartida_ComReservas_DevePersistirComTransacao()
    {
        await using var fixture = await IntegrationFixture.CreateAsync();

        var partida = await fixture.PartidaService.SalvarPartidaAsync(new SalvarPartidaDTO
        {
            TimeA = new SalvarTimeDTO { Nome = "Time A", AtletaIds = [1, 2, 3, 4] },
            TimeB = new SalvarTimeDTO { Nome = "Time B", AtletaIds = [5, 6, 7, 8] },
            ReservaIds = [9, 10]
        });

        Assert.True(partida.ID > 0);
        Assert.Equal(2, partida.Reservas.Count);
    }

    [Fact]
    public async Task RecarregarPartida_DeveTrazerTimesAtletasEReservas()
    {
        await using var fixture = await IntegrationFixture.CreateAsync();

        var partidaCriada = await fixture.PartidaService.SalvarPartidaAsync(new SalvarPartidaDTO
        {
            TimeA = new SalvarTimeDTO { Nome = "Time A", AtletaIds = [1, 2, 3, 4] },
            TimeB = new SalvarTimeDTO { Nome = "Time B", AtletaIds = [5, 6, 7, 8] },
            ReservaIds = [9, 10]
        });

        var recarregada = await fixture.PartidaService.GetByIdAsync(partidaCriada.ID);

        Assert.NotNull(recarregada);
        Assert.Equal(4, recarregada!.TimeA.Atletas.Count);
        Assert.Equal(4, recarregada.TimeB.Atletas.Count);
        Assert.Equal(2, recarregada.Reservas.Count);
    }

    [Fact]
    public async Task RegistrarResultado_ERecuperarSubstituicoes_DeveFuncionar()
    {
        await using var fixture = await IntegrationFixture.CreateAsync();

        var partidaCriada = await fixture.PartidaService.SalvarPartidaAsync(new SalvarPartidaDTO
        {
            TimeA = new SalvarTimeDTO { Nome = "Time A", AtletaIds = [1, 2, 3, 4] },
            TimeB = new SalvarTimeDTO { Nome = "Time B", AtletaIds = [5, 6, 7, 8] },
            ReservaIds = [9, 10]
        });

        var partidaEncerrada = await fixture.PartidaService.RegistrarResultadoAsync(partidaCriada.ID, new RegistrarResultadoPartidaDTO
        {
            TimeVencedorId = partidaCriada.TimeA.ID
        });

        Assert.Single(partidaEncerrada.Substituicoes);

        var substituicoes = await fixture.PartidaService.GetSubstituicoesAsync();
        Assert.Single(substituicoes);

        var substituicao = await fixture.PartidaService.GetSubstituicaoByIdAsync(substituicoes[0].Id);
        Assert.NotNull(substituicao);
    }

    [Fact]
    public async Task SalvarPartida_QuandoDuplicarAtleta_DeveFalhar()
    {
        await using var fixture = await IntegrationFixture.CreateAsync();

        var act = () => fixture.PartidaService.SalvarPartidaAsync(new SalvarPartidaDTO
        {
            TimeA = new SalvarTimeDTO { Nome = "Time A", AtletaIds = [1, 2, 3, 4] },
            TimeB = new SalvarTimeDTO { Nome = "Time B", AtletaIds = [4, 5, 6, 7] },
            ReservaIds = [8, 9]
        });

        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task SQLite_IdsEFks_DeveManterIntegridade()
    {
        await using var fixture = await IntegrationFixture.CreateAsync();

        var partida = await fixture.PartidaService.SalvarPartidaAsync(new SalvarPartidaDTO
        {
            TimeA = new SalvarTimeDTO { Nome = "Time A", AtletaIds = [1, 2, 3, 4] },
            TimeB = new SalvarTimeDTO { Nome = "Time B", AtletaIds = [5, 6, 7, 8] },
            ReservaIds = [9, 10]
        });

        var persisted = await fixture.Context.Partidas
            .Include(p => p.Reservas)
            .FirstAsync(p => p.ID == partida.ID);

        Assert.True(persisted.TimeAId > 0);
        Assert.True(persisted.TimeBId > 0);
        Assert.NotNull(await fixture.Context.Times.FindAsync(persisted.TimeAId));
        Assert.NotNull(await fixture.Context.Times.FindAsync(persisted.TimeBId));
        Assert.All(persisted.Reservas, r => Assert.True(r.AtletaId > 0));

        await using var command = fixture.Context.Database.GetDbConnection().CreateCommand();
        command.CommandText = "PRAGMA foreign_key_list('PartidaReservas');";

        if (command.Connection is not null && command.Connection.State != System.Data.ConnectionState.Open)
            await command.Connection.OpenAsync();

        var fkTables = new List<string>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            fkTables.Add(reader.GetString(2));

        Assert.Contains("Partidas", fkTables);
        Assert.Contains("Atletas", fkTables);
    }

    private sealed class IntegrationFixture : IAsyncDisposable
    {
        public VoleiContext Context { get; }
        public PartidaService PartidaService { get; }

        private IntegrationFixture(VoleiContext context, PartidaService partidaService)
        {
            Context = context;
            PartidaService = partidaService;
        }

        public static async Task<IntegrationFixture> CreateAsync()
        {
            var dbPath = Path.Combine(Path.GetTempPath(), $"voleiapp-integration-{Guid.NewGuid():N}.db");
            var options = new DbContextOptionsBuilder<VoleiContext>()
                .UseSqlite($"Data Source={dbPath};Foreign Keys=True")
                .Options;

            var context = new VoleiContext(options);
            await context.Database.MigrateAsync();

            context.Atletas.AddRange(
                new Atleta { ID = 1, Nome = "A1", Posicao = EPosicao.Ponteiro },
                new Atleta { ID = 2, Nome = "A2", Posicao = EPosicao.Oposto },
                new Atleta { ID = 3, Nome = "A3", Posicao = EPosicao.Central },
                new Atleta { ID = 4, Nome = "A4", Posicao = EPosicao.Levantador },
                new Atleta { ID = 5, Nome = "A5", Posicao = EPosicao.Ponteiro },
                new Atleta { ID = 6, Nome = "A6", Posicao = EPosicao.Oposto },
                new Atleta { ID = 7, Nome = "A7", Posicao = EPosicao.Central },
                new Atleta { ID = 8, Nome = "A8", Posicao = EPosicao.Levantador },
                new Atleta { ID = 9, Nome = "R1", Posicao = EPosicao.Central },
                new Atleta { ID = 10, Nome = "R2", Posicao = EPosicao.Levantador }
            );
            await context.SaveChangesAsync();

            var atletaRepository = new AtletaRepository(context);
            var partidaRepository = new PartidaRepository(context);
            var sorteioService = new SorteioService();
            var partidaService = new PartidaService(atletaRepository, sorteioService, partidaRepository);

            return new IntegrationFixture(context, partidaService);
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
        }
    }
}
