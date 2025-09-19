using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Models
{
    public class Korisnik
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        //public List<Figura> Figure { get; set; }
        public Figura Figura { get; set; }
        public int Start { get; set; }
        public int Cilj { get; set; }

        public Korisnik(int id, string ime, int start, int cilj)
        {
            Id = id;
            Ime = ime;
            Start = start;
            Cilj = cilj;
            //Figure = new List<Figura>();
            Figura = new Figura();
        }

        

    }
}
