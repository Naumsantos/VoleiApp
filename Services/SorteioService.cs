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
        public List<Time> SortearTimes(List<Atleta> atletas, SorteioConfig config, out Queue<Atleta> reservas)
        {
            var embaralhado = atletas.OrderBy(a => _random.Next()).ToList();
            var times = new List<Time>();

            for (var i = 0; i < embaralhado.Count; i++)
                times.Add(new Time { 
                                ID = i + 1, 
                                Nome = $"Time A" 
                });

            int t = 0;
            int totalJogadores = config.NumeroDeTimes * config.JogadorPorTimes;
            for (var i = 0; i < totalJogadores && i < embaralhado.Count; i++)
            {
                times[t].Atletas.Add(embaralhado[i]); //adiciona o jogador no time
                t = (t + 1) % config.NumeroDeTimes;
            }

            reservas = new Queue<Atleta>(embaralhado.Skip(totalJogadores));
            
            return times;
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
                TimeID = timePerdedor.ID,
                Entraram = entraram,
                Sairam = sairam
            };
        }

    }
}
