using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoleiApp.Models
{
    public class Substituicao
    {
        public int TimeID { get; set; }
        public List<Atleta> Entraram { get; set; } = new List<Atleta>();
        public List<Atleta> Sairam { get; set; } = new List<Atleta>();
    }
}
