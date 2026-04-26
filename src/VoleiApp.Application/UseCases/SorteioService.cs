using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoleiApp.Models;

namespace VoleiApp.Services
{
    public class SorteioService
    {
        readonly Random _random = new();
        public SorteioResultDTO SortearTimes(SorteioConfigDTO config)
        {
            var embaralhados = config.Atletas.OrderBy(_ => Guid.NewGuid()).ToList();
            var times = new List<Time>();
            var reservas = new List<Atleta>();

            int qtdPorTime = config.TamanhoDoTime;
            int totalTimes = embaralhados.Count / qtdPorTime;

            for (int i = 0; i < totalTimes; i++)
            {
                var atletasDoTime = embaralhados.Skip(i * qtdPorTime).Take(qtdPorTime).ToList();
                times.Add(new Time
                {
                    Nome = $"Time {i + 1}",
                    Atletas = atletasDoTime
                });
            }

            // Reservas: o que sobrou depois de formar os times
            reservas = embaralhados.Skip(totalTimes * qtdPorTime).ToList();

            return new SorteioResultDTO
            {
                Times = times,
                Reservas = reservas
            };
        }

        public Substituicao SubstituirJogadores(Time timePerdedor, Queue<Atleta> reservas, int qtdSubstituicoes = 1)
        {
            var sairam = timePerdedor.Atletas.OrderBy(a => _random.Next()).Take((int)qtdSubstituicoes).ToList();
            var entraram = new List<Atleta>();

            foreach (var item in sairam) 
            { 
                if (reservas.Count == 0) break;
                var novo = reservas.Dequeue();
                entraram.Add(novo);
                timePerdedor.Atletas.Remove(item);
                timePerdedor.Atletas.Add(novo);
                reservas.Enqueue(item);
            }

            return new Substituicao
            {
                Id = timePerdedor.ID,
                Entraram = entraram,
                Sairam = sairam
            };
        }

    }
}
