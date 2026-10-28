using VoleiApp.Application.DTOs.Atleta;
using VoleiApp.Application.DTOs.Sorteio;
using VoleiApp.Application.Interfaces.Services;
using VoleiApp.Domain.Entities;
using VoleiApp.Domain.Enums;
using VoleiApp.Domain.Interfaces.Repositories;

namespace VoleiApp.Application.Services
{
    public class SorteioService : ISorteioService
    {
        private readonly IAtletaRepository? _atletaRepository;
        private readonly ITimeRepository? _timeRepository;

        static readonly EPosicao[] _atacantes = { EPosicao.Ponteiro, EPosicao.Oposto };
        static readonly EPosicao[] _meios = { EPosicao.Central };
        static readonly EPosicao[] _levantadores = { EPosicao.Levantador };

        public SorteioService()
        {
        }

        public SorteioService(IAtletaRepository atletaRepository)
        {
            _atletaRepository = atletaRepository;
        }

        public SorteioService(IAtletaRepository atletaRepository, ITimeRepository timeRepository)
        {
            _atletaRepository = atletaRepository;
            _timeRepository = timeRepository;
        }

        public async Task<SorteioResultDTO> SortearTimes(SorteioConfigDTO config)
        {
            if (config.AtacantesPorTime + config.LevantadoresPorTime + config.MeiosPorTime != config.TamanhoDoTime)
            {
                return new SorteioResultDTO
                {
                    Warnings = new List<string> { "Configuração inválida: a soma das posições deve ser igual ao TamanhoDoTime." }
                };
            }

            var atletas = new AtletasDisponiveisDTO { Atletas = config.Atletas };

            var atletasDisponiveis = await ObterAtletasDisponiveisAsync(atletas);
            if (atletasDisponiveis.Count == 0)
            {
                return new SorteioResultDTO
                {
                    Warnings = new List<string> { "Não há atletas cadastrados para o sorteio." }
                };
            }

            var resultado = new SorteioResultDTO();
            var todos = Embaralhar(atletasDisponiveis);
            var atacantes = new Queue<Atleta>(todos.Where(a => _atacantes.Contains(a.Posicao)));
            var meios = new Queue<Atleta>(todos.Where(a => _meios.Contains(a.Posicao)));
            var levantadores = new Queue<Atleta>(todos.Where(a => _levantadores.Contains(a.Posicao)));

            while (atacantes.Count >= config.AtacantesPorTime &&
                   meios.Count >= config.MeiosPorTime &&
                   levantadores.Count >= config.LevantadoresPorTime)
            {
                var atletasTime = new List<TimeAtleta>();
                atletasTime.AddRange(Retirar(atacantes, config.AtacantesPorTime));
                atletasTime.AddRange(Retirar(meios, config.MeiosPorTime));
                atletasTime.AddRange(Retirar(levantadores, config.LevantadoresPorTime));

                resultado.Times.Add(new Time
                {
                    Nome = $"Time {resultado.Times.Count + 1}",
                    Atletas = atletasTime
                });
            }

            var atletasNaoAlocados = CriarTimesIncompletos(resultado, config, atacantes, meios, levantadores);

            resultado.Reservas = atletasNaoAlocados;

            resultado.TimesIncompletos = resultado.Times.Count(t => !TimeEhIdeal(t, config));

            if (resultado.TimesIncompletos > 0)
                resultado.Warnings.Add($"{resultado.TimesIncompletos} time(s) incompleto(s) aguardando posições faltantes.");

            await PersistirTimesAsync(resultado.Times);

            return resultado;
        }

        public Task<Substituicao> SubstituirJogadores(Time timePerdedor, Queue<Atleta> reservas)
        {
            var sairam = new List<Atleta>();
            var entraram = new List<Atleta>();

            var reservasPorPosicao = reservas
                .GroupBy(a => a.Posicao)
                .ToDictionary(g => g.Key, g => new Queue<Atleta>(g));

            foreach (var (posicao, filaReservas) in reservasPorPosicao)
            {
                var titulares = timePerdedor.Atletas.Where(ta => ta.Atleta.Posicao == posicao).ToList();

                foreach (var titularTA in titulares)
                {
                    if (filaReservas.Count == 0) break;

                    var reserva = filaReservas.Dequeue();

                    entraram.Add(reserva);
                    sairam.Add(titularTA.Atleta);

                    // substituir TimeAtleta
                    timePerdedor.Atletas.Remove(titularTA);
                    timePerdedor.Atletas.Add(new TimeAtleta { ID = reserva.ID, Atleta = reserva, AtletaId = reserva.ID });

                    // coloca os titulares substituídos na lista de reservas
                    var listaReservas = reservas.ToList();
                    listaReservas.Remove(reserva);
                    listaReservas.Add(titularTA.Atleta);

                    reservas.Clear();
                    foreach (var r in listaReservas)
                        reservas.Enqueue(r);
                }
            }
            return Task.FromResult(new Substituicao
            {
                Entraram = entraram,
                Sairam = sairam
            });
        }

        private async Task<List<Atleta>> ObterAtletasDisponiveisAsync(AtletasDisponiveisDTO config)
        {
            if (_atletaRepository is not null)
                return await _atletaRepository.GetAllAsync();

            return config.Atletas ?? new List<Atleta>();
        }

        private async Task PersistirTimesAsync(IEnumerable<Time> times)
        {
            if (_timeRepository is null)
                return;

            foreach (var time in times)
            {
                var entidade = new Time
                {
                    Nome = time.Nome,
                    Atletas = time.Atletas
                        .Select(ta => new TimeAtleta { AtletaId = ta.AtletaId })
                        .ToList()
                };

                await _timeRepository.AddAsync(entidade, CancellationToken.None);
            }
        }

        private static List<Atleta> CriarTimesIncompletos(
            SorteioResultDTO resultado,
            SorteioConfigDTO config,
            Queue<Atleta> atacantes,
            Queue<Atleta> meios,
            Queue<Atleta> levantadores)
        {
            var atletasNaoAlocados = new List<Atleta>();

            if (atacantes.Count + meios.Count + levantadores.Count == 0)
                return atletasNaoAlocados;

            var atletasTime = new List<TimeAtleta>();
            atletasTime.AddRange(Retirar(atacantes, config.AtacantesPorTime));
            atletasTime.AddRange(Retirar(meios, config.MeiosPorTime));
            atletasTime.AddRange(Retirar(levantadores, config.LevantadoresPorTime));

            if (atletasTime.Count > 0)
            {
                var time = new Time
                {
                    Nome = $"Time {resultado.Times.Count + 1}",
                    Atletas = atletasTime
                };

                resultado.Times.Add(time);

                if (!TimeEhIdeal(time, config))
                {
                    var faltando = PosicoesFaltantes(time, config);
                    resultado.Warnings.Add($"{time.Nome} incompleto. Faltando: {faltando}.");
                }
            }

            atletasNaoAlocados.AddRange(atacantes);
            atletasNaoAlocados.AddRange(meios);
            atletasNaoAlocados.AddRange(levantadores);

            return atletasNaoAlocados;
        }

        private static string PosicoesFaltantes(Time time, SorteioConfigDTO config)
        {
            var faltas = new List<string>();

            var atacantesNoTime = time.Atletas.Count(a => _atacantes.Contains(a.Atleta.Posicao));
            var meiosNoTime = time.Atletas.Count(a => _meios.Contains(a.Atleta.Posicao));
            var levantadoresNoTime = time.Atletas.Count(a => _levantadores.Contains(a.Atleta.Posicao));

            if (atacantesNoTime < config.AtacantesPorTime)
                faltas.Add($"{config.AtacantesPorTime - atacantesNoTime} atacante(s)");

            if (meiosNoTime < config.MeiosPorTime)
                faltas.Add($"{config.MeiosPorTime - meiosNoTime} central(is)");

            if (levantadoresNoTime < config.LevantadoresPorTime)
                faltas.Add($"{config.LevantadoresPorTime - levantadoresNoTime} levantador(es)");

            return string.Join(", ", faltas);
        }

        private static bool TimeEhIdeal(Time time, SorteioConfigDTO config)
        {
            int atac = time.Atletas.Count(a => _atacantes.Contains(a.Atleta.Posicao));
            int meio = time.Atletas.Count(m => _meios.Contains(m.Atleta.Posicao));
            int levantador = time.Atletas.Count(l => _levantadores.Contains(l.Atleta.Posicao));

            return atac == config.AtacantesPorTime &&
                   meio == config.MeiosPorTime &&
                   levantador == config.LevantadoresPorTime &&
                   time.Atletas.Count == config.TamanhoDoTime;
        }

        private static List<TimeAtleta> Retirar(Queue<Atleta> pool, int qtd)
        {
            var escolhidos = new List<TimeAtleta>();

            for (var i = 0; i < qtd && pool.Count > 0; i++)
            {
                var atleta = pool.Dequeue();
                escolhidos.Add(new TimeAtleta { ID = atleta.ID, Atleta = atleta, AtletaId = atleta.ID });
            }

            return escolhidos;
        }

        private static List<Atleta> Embaralhar(IEnumerable<Atleta> source) => source.OrderBy(_ => Guid.NewGuid()).ToList();
    }
}
