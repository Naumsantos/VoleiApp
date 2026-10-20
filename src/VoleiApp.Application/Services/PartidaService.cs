using VoleiApp.Application.DTOs.Partida;
using VoleiApp.Application.Interfaces.Services;
using VoleiApp.Domain.Entities;
using VoleiApp.Domain.Interfaces.Repositories;

namespace VoleiApp.Application.Services
{
    public class PartidaService : IPartidaService
    {
        private readonly IPartidaRepository _partidaRepository;
        private readonly ISorteioService _sorteioService;
        private readonly IAtletaRepository _atletaRepository;

        public PartidaService(
            IAtletaRepository atletaRepository,
            ISorteioService sorteioService,
            IPartidaRepository partidaRepository)
        {
            _atletaRepository = atletaRepository;
            _partidaRepository = partidaRepository;
            _sorteioService = sorteioService;
        }

        public async Task<PartidaResponseDTO> CreateAsync(CriarPartidaDTO dto, CancellationToken ct = default)
        {
            var partida = await MontarPartidaAsync(dto.TimeA.Nome, dto.TimeA.AtletaIds, dto.TimeB.Nome, dto.TimeB.AtletaIds, dto.ReservaIds, ct);

            await _partidaRepository.SavePartidaGraphAsync(partida, ct);

            var persisted = await _partidaRepository.GetByIdAsync(partida.ID, ct)
                ?? throw new InvalidOperationException("Partida não encontrada após a criação.");

            return ToResponse(persisted);
        }

        public async Task<PartidaResponseDTO> SalvarPartidaAsync(SalvarPartidaDTO dto, CancellationToken ct = default)
        {
            var partida = await MontarPartidaAsync(dto.TimeA.Nome, dto.TimeA.AtletaIds, dto.TimeB.Nome, dto.TimeB.AtletaIds, dto.ReservaIds, ct);

            await _partidaRepository.SavePartidaGraphAsync(partida, ct);

            var persisted = await _partidaRepository.GetByIdAsync(partida.ID, ct)
                ?? throw new InvalidOperationException("Partida não encontrada após persistência.");

            return ToResponse(persisted);
        }

        public async Task<List<PartidaResponseDTO>> GetAllAsync(CancellationToken ct = default)
        {
            return (await _partidaRepository.GetAllAsync(ct)).Select(ToResponse).ToList();
        }

        public async Task<PartidaResponseDTO?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var partida = await _partidaRepository.GetByIdAsync(id, ct);

            return partida is null ? null : ToResponse(partida);
        }

        public async Task<List<SubstituicaoResponseDTO>> GetSubstituicoesAsync(CancellationToken ct = default)
        {
            var substituicoes = await _partidaRepository.GetSubstituicoesAsync(ct);
            return substituicoes.Select(ToSubstituicaoResponse).ToList();
        }

        public async Task<SubstituicaoResponseDTO?> GetSubstituicaoByIdAsync(int id, CancellationToken ct = default)
        {
            var substituicao = await _partidaRepository.GetSubstituicaoByIdAsync(id, ct);
            return substituicao is null ? null : ToSubstituicaoResponse(substituicao);
        }

        public async Task<PartidaResponseDTO> RegistrarResultadoAsync(int partidaId, RegistrarResultadoPartidaDTO dto, CancellationToken ct = default)
        {
            var partida = await _partidaRepository.GetByIdAsync(partidaId, ct)
                ?? throw new KeyNotFoundException($"Partida {partidaId} não encontrada.");

            if (partida.Encerrada)
                throw new InvalidOperationException("Partida já encerrada.");

            if (dto.TimeVencedorId != partida.TimeAId && dto.TimeVencedorId != partida.TimeBId)
                throw new ArgumentException("O time vencedor informado não pertence à partida.");

            partida.TimeVencedorId = dto.TimeVencedorId;
            partida.Encerrada = true;

            var timePerdedor = dto.TimeVencedorId == partida.TimeAId ? partida.TimeB : partida.TimeA;

            var filaReservas = new Queue<Atleta>(
                partida.Reservas
                    .OrderBy(r => r.Ordem)
                    .Select(r => r.Atleta));

            var timePerdedorTemp = new Time
            {
                ID = timePerdedor.ID,
                Nome = timePerdedor.Nome,
                Atletas = timePerdedor.Atletas
                    .Select(ta => new TimeAtleta
                    {
                        TimeId = timePerdedor.ID,
                        AtletaId = ta.AtletaId,
                        Atleta = ta.Atleta
                    })
                    .ToList()
            };

            var substituicao = await _sorteioService.SubstituirJogadores(timePerdedorTemp, filaReservas);

            timePerdedor.Atletas.Clear();
            foreach (var ta in timePerdedorTemp.Atletas)
            {
                timePerdedor.Atletas.Add(new TimeAtleta
                {
                    TimeId = timePerdedor.ID,
                    AtletaId = ta.AtletaId,
                    Atleta = ta.Atleta
                });
            }

            partida.Reservas.Clear();
            int ordem = 0;
            foreach (var atleta in filaReservas)
            {
                partida.Reservas.Add(new PartidaReserva
                {
                    PartidaId = partida.ID,
                    AtletaId = atleta.ID,
                    Atleta = atleta,
                    Ordem = ordem++
                });
            }

            partida.Substituicoes.Add(new Substituicao
            {
                PartidaId = partida.ID,
                TimeId = timePerdedor.ID,
                Time = timePerdedor,
                Entraram = substituicao.Entraram,
                Sairam = substituicao.Sairam
            });

            await _partidaRepository.UpdateAsync(partida, ct);

            var atualizada = await _partidaRepository.GetByIdAsync(partida.ID, ct)
                ?? throw new InvalidOperationException("Partida não encontrada após atualização.");

            return ToResponse(atualizada);
        }

        private async Task<Partida> MontarPartidaAsync(
            string nomeTimeA,
            IEnumerable<int> idsTimeA,
            string nomeTimeB,
            IEnumerable<int> idsTimeB,
            IEnumerable<int> idsReservas,
            CancellationToken ct)
        {
            var atletasTimeA = await BuscarAtletas(idsTimeA, ct);
            var atletasTimeB = await BuscarAtletas(idsTimeB, ct);
            var atletasReservas = await BuscarAtletas(idsReservas, ct);

            ValidarDuplicidade(atletasTimeA, atletasTimeB, atletasReservas);

            var timeA = new Time
            {
                Nome = nomeTimeA,
                Atletas = atletasTimeA.Select(a => new TimeAtleta { AtletaId = a.ID, Atleta = a }).ToList()
            };

            var timeB = new Time
            {
                Nome = nomeTimeB,
                Atletas = atletasTimeB.Select(a => new TimeAtleta { AtletaId = a.ID, Atleta = a }).ToList()
            };

            return new Partida
            {
                TimeA = timeA,
                TimeB = timeB,
                Reservas = atletasReservas.Select((a, index) =>
                    new PartidaReserva
                    {
                        AtletaId = a.ID,
                        Atleta = a,
                        Ordem = index
                    }).ToList(),
                Substituicoes = []
            };
        }

        private async Task<List<Atleta>> BuscarAtletas(IEnumerable<int> ids, CancellationToken ct)
        {
            var atletas = new List<Atleta>();

            foreach (var id in ids.Distinct())
            {
                var atleta = await _atletaRepository.GetByIdAsync(id, ct)
                    ?? throw new KeyNotFoundException($"Atleta {id} não encontrado");

                atletas.Add(atleta);
            }

            return atletas;
        }

        private static void ValidarDuplicidade(List<Atleta> timeA, List<Atleta> timeB, List<Atleta> reservas)
        {
            var ids = timeA.Select(a => a.ID)
                .Concat(timeB.Select(b => b.ID))
                .Concat(reservas.Select(r => r.ID))
                .ToList();

            if (ids.Count != ids.Distinct().Count())
                throw new ArgumentException("Um mesmo atleta nao pode estar em times distintos ao mesmo tempo.");
        }

        private static SubstituicaoResponseDTO ToSubstituicaoResponse(Substituicao substituicao) => new()
        {
            Id = substituicao.Id,
            TimeId = substituicao.TimeId,
            Data = substituicao.Data,
            Entraram = substituicao.Entraram.Select(ToAtletaResumo).ToList(),
            Sairam = substituicao.Sairam.Select(ToAtletaResumo).ToList()
        };

        private static PartidaResponseDTO ToResponse(Partida partida) => new()
        {
            ID = partida.ID,
            Data = partida.Data,
            Encerrada = partida.Encerrada,
            TimeVencedorId = partida.TimeVencedorId,
            TimeA = ToTimeResponse(partida.TimeA),
            TimeB = ToTimeResponse(partida.TimeB),
            Reservas = partida.Reservas
                .OrderBy(r => r.Ordem)
                .Select(r => ToAtletaResumo(r.Atleta))
                .ToList(),
            Substituicoes = partida.Substituicoes.Select(ToSubstituicaoResponse).ToList()
        };

        private static TimeResponseDTO ToTimeResponse(Time time) => new()
        {
            ID = time.ID,
            Nome = time.Nome,
            Atletas = time.Atletas.Select(ta => ToAtletaResumo(ta.Atleta)).ToList()
        };

        private static AtletaResumoDTO ToAtletaResumo(Atleta atleta) => new()
        {
            ID = atleta.ID,
            Nome = atleta.Nome,
            Posicao = atleta.Posicao
        };
    }
}
