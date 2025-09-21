using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Models
{
    public class Igra
    {
        //public int BrojPoljaTable {  get; set; }
        public List<Korisnik> Igraci {  get; set; }
        public int TrenutniIgrac { get; set; }
        public bool KrajIgre { get; set; }

        public Igra(List<Korisnik> igraci)
        {
            Igraci = igraci;
            TrenutniIgrac = 0;
            KrajIgre = false;
        }

        // Koji je trenutni igrac
        public Korisnik VratiTrenutnogIgraca()
        {
            return Igraci[TrenutniIgrac];
        }

        public void SledeciIgrac()
        {
            TrenutniIgrac = (TrenutniIgrac + 1) % Igraci.Count;
        }

    }
}

