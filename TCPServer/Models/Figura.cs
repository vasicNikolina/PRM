using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Models
{
    public class Figura
    {
        public bool Aktivna {  get; set; }
        public int TrenutnaPozicija { get; set; }
        public int UdaljenostDoCilja { get; set; }

        public Figura() 
        {
            Aktivna = false;
            TrenutnaPozicija = -1;
            UdaljenostDoCilja = 0;
        }

        public void Pomeraj(int pomeraj)
        {
            TrenutnaPozicija += pomeraj;
            if(UdaljenostDoCilja - pomeraj < 0)
            {
                UdaljenostDoCilja = 0;
            }
            else
            {
                UdaljenostDoCilja -= pomeraj;
            }
        }

    }
}
