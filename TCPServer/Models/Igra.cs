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
        /*
        public void ProveriZavrsetakIgre()
        {
            foreach(var igrac in Igraci)
            {
                bool sveSuFigureNaCilju = true; //pretpostavljamo

                foreach (var figura in igrac.Figura)
                {
                    if(figura.UdaljenostDoCilja != 0)
                    {
                        sveSuFigureNaCilju = false;
                        break;
                    }
                }
                if (sveSuFigureNaCilju)
                {
                    KrajIgre = true;
                    break;
                }
            }   
        }*/

        /*public void ProveriZavrsetakIgre()
        {
            foreach(var igrac in Igraci)
            {
                if(igrac.Figure.Count(f=> f.UdaljenostDoCilja == 0) == igrac.Figure.Count){
                    KrajIgre = true;
                    break;
                }
            }

        }*/

    }
}

/*public Igra(int poljaTable)
        {
            if(poljaTable < 16 || poljaTable % 4 != 0)
            {
                throw new ArgumentException("Broj polja na tabli mora biti deljiv sa 4 i veci od 16!");
            }
            BrojPoljaTable = poljaTable;
            Igraci = new List<Korisnik>();
            TrenutniIgracIndex = 0;
            KrajIgre = false;
        }*/

/*public void DodajIgraca(Korisnik igrac)
        {
            Igraci.Add(igrac);
        }*/
