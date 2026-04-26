using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoleiApp.Models
{
    public class Time
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public  List<Atleta> Atletas { get; set; }
    }
}
