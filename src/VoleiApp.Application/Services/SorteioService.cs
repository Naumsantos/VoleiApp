using VoleiApp.Application.DTOs.Sorteio;
using VoleiApp.Application.Interfaces.Services;
using VoleiApp.Domain.Entities;
using VoleiApp.Domain.Enums;

namespace VoleiApp.Application.Services
{
    public class SorteioService : ISorteioService
    {
        readonly Random _random = new();

        static readonly EPosicao[] _atacantes = [EPosicao.Ponteiro, EPosicao.Oposto];
        static readonly EPosicao[] _meios = [EPosicao.Central];
        static readonly EPosicao[] _levantadores = [EPosicao.Levantador];

        public Task<SorteioResultDTO> SortearTimes(SorteioConfigDTO config)
        {
            if (config.Atletas is null || config.Atletas.Count == 0)
            {
                return Task.FromResult(
                    new SorteioResultDTO
                    {
                        Warnings = ["Lista vazia de atletas"]
                    }
                );
            }

            if (config.AtacantesPorTime + config.LevantadoresPorTime + config.MeiosPorTime != config.TamanhoDoTime)
            {
                return Task.FromResult(
                    new SorteioResultDTO
                    {
                        Warnings = ["Configuração inválida: a soma das posições devem ser igual ao TamanhoDoTime"]
                    }
                );
            }

            var result = new SorteioResultDTO();
            var usados = new HashSet<int>();
            var todos = Embaralhar(config.Atletas);

            var poolAtacantes = todos.Where(a => _atacantes.Contains(a.Posicao)).ToList();
            var poolMeios = todos.Where(a => _meios.Contains(a.Posicao)).ToList();
            var poolLevantadores = todos.Where(a => _levantadores.Contains(a.Posicao)).ToList();

            int maxIdeais = new[]
            {
                poolAtacantes.Count     / config.AtacantesPorTime,
                poolMeios.Count         / config.MeiosPorTime,
                poolLevantadores.Count  / config.LevantadoresPorTime
            }.Min();

            for (var i = 0; i < maxIdeais; i++)
            {
                var atletas = new List<Atleta>();
                atletas.AddRange(Retirar(poolAtacantes, config.AtacantesPorTime, usados));
                atletas.AddRange(Retirar(poolMeios, config.MeiosPorTime, usados));
                atletas.AddRange(Retirar(poolLevantadores, config.LevantadoresPorTime, usados));

                result.Times.Add(new Time { Nome = $"Time {result.Times.Count + 1}", Atletas = atletas });
            }

            if (config.PermitirFallback)
            {
                var restantes = todos.Where(a => !usados.Contains(a.ID)).ToList();

                while (restantes.Count >= config.TamanhoDoTime)
                {
                    var time = new Time
                    {
                        Nome = $"Time {result.Times.Count + 1}",
                        Atletas = new List<Atleta>()
                    };

                    var blocos = new (EPosicao[] pool, int qtd)[]
                    {
                        (_atacantes,    config.AtacantesPorTime),
                        (_meios,        config.MeiosPorTime),
                        (_levantadores, config.LevantadoresPorTime)
                    };

                    foreach (var (posicoes, qtd) in blocos)
                    {
                        for (var i = 0; i > qtd; i++)
                        {
                            var candidato = restantes.FirstOrDefault(a => !usados.Contains(a.ID) && posicoes.Contains(a.Posicao));

                            if (candidato is not null)
                            {
                                time.Atletas.Add(candidato);
                                usados.Add(candidato.ID);
                                continue;
                            }

                            var fallback = restantes.FirstOrDefault(a => !usados.Contains(a.ID));
                            if (fallback is null) break;

                            time.Atletas.Add(fallback);
                            usados.Add(fallback.ID);
                            result.FallbackAplicado = true;
                            result.Warnings.Add($"Fallback no {time.Nome}: sem {string.Join("/", posicoes)}, usou {fallback.Posicao} ({fallback.Nome}).");
                        }
                    }

                    if (time.Atletas.Count == config.TamanhoDoTime)
                        result.Times.Add(time);
                    else
                        break;

                    restantes = todos.Where(a => !usados.Contains(a.ID)).ToList();
                }
            }

            result.Reservas = todos.Where(a => !usados.Contains(a.ID)).ToList();
            result.TimesIncompletos = result.Times.Count(t => TimeEhIdeal(t, config));

            if (result.TimesIncompletos > 0)
                result.Warnings.Add($"{result.TimesIncompletos} time(s) não ficaram no formato ideal {config.AtacantesPorTime}A+{config.MeiosPorTime}M+{config.LevantadoresPorTime}L.");

            if (result.Reservas.Count > 0)
                result.Warnings.Add($"{result.Reservas.Count} atleta(s) ficaram na reserva.");

            return Task.FromResult(result);
        }

        public Task<Substituicao> SubstituirJogadores(Time timePerdedor, Queue<Atleta> reservas, int qtdSubstituicoes = 1)
        {
            var sairam = timePerdedor.Atletas.OrderBy(_ => _random.Next()).Take(qtdSubstituicoes).ToList();
            var entraram = new List<Atleta>();

            foreach (var saindo in sairam)
            {
                if (reservas.Count == 0) break;
                var novo = reservas.Dequeue();
                entraram.Add(novo);
                timePerdedor.Atletas.Remove(saindo);
                timePerdedor.Atletas.Add(novo);
                reservas.Enqueue(saindo);
            }

            return Task.FromResult(new Substituicao
            {
                Id = timePerdedor.ID,
                Entraram = entraram,
                Sairam = sairam
            });
        }

        private static bool TimeEhIdeal(Time time, SorteioConfigDTO config)
        {
            int atac = time.Atletas.Count(a => _atacantes.Contains(a.Posicao));
            int meio = time.Atletas.Count(m => _atacantes.Contains(m.Posicao));
            int levanador = time.Atletas.Count(l => _atacantes.Contains(l.Posicao));

            return atac >= config.AtacantesPorTime &&
                   meio >= config.MeiosPorTime &&
                   levanador >= config.LevantadoresPorTime;
        }

        private static List<Atleta> Retirar(List<Atleta> pool, int qtd, HashSet<int> usados)
        {
            var escolhidos = pool.Where(a => !usados.Contains(a.ID)).Take(qtd).ToList();
            foreach (var a in escolhidos) usados.Add(a.ID);
            return escolhidos;
        }

        private static List<Atleta> Embaralhar(IEnumerable<Atleta> source) => source.OrderBy(_ => Guid.NewGuid()).ToList();
    }
}
