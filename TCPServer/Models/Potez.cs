using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Models
{
    public enum Akcija { Aktivacija, Pomeranje, Deaktivacija}
    public class Potez
    {
        public Figura Figura { get; set; }
        public Akcija Akcija { get; set; }
        public int BrojPolja {  get; set; }

        public Potez(Figura figura, Akcija akcija, int brojPolja)
        {
            Figura = figura;
            Akcija = akcija;
            BrojPolja = brojPolja;
        }
    }
}
