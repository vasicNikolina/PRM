using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TCPServer.Models;

namespace TCPServer
{
    internal class Server
    {
        private static List<Socket> clients = new List<Socket>();
        private static int poljaTable = 20;
        private static Igra igra;

        static void Main(string[] args)
        {
            Console.WriteLine("Unesite broj igraca: ");
            int brojIgraca = int.Parse(Console.ReadLine());

            // Igraci i njihove figure
            List<Korisnik> igraci = new List<Korisnik>();

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 50001);
            serverSocket.Bind(serverEP);
            serverSocket.Listen(brojIgraca);

            Console.WriteLine("Server ceka i slusa igrace.");

            for (int i = 0; i < brojIgraca; i++)
            {
                Socket acceptedSocket = serverSocket.Accept();
                clients.Add(acceptedSocket);
                IPEndPoint clientEP = acceptedSocket.RemoteEndPoint as IPEndPoint;
                Console.WriteLine($"Igrac {i + 1} je povezan: {clientEP}");

                // Kreiranje figure za korisnika
                Korisnik igrac = new Korisnik(i + 1, "Igrac" + (i + 1), 0, poljaTable);
                igraci.Add(igrac);
            }

            // Inicijalizacija igre
            igra = new Igra(igraci);
            Console.WriteLine("Svi igraci su povezani, POCINJEMO.");

            
            try
            {
                while (!igra.KrajIgre)
                {
                    Korisnik igracTrenutni = igra.VratiTrenutnogIgraca();
                    Socket trenutniSocket = clients[igra.TrenutniIgrac];

                    // Obavestavamo igraca da je on na redu
                    byte[] poruka = Encoding.UTF8.GetBytes("Vas je red! Bacite kockicu, pritisnite enter.");
                    trenutniSocket.Send(poruka);

                    // Prima se odgovor od igraca
                    byte[] buffer = new byte[1024];
                    int brBajtova = trenutniSocket.Receive(buffer);
                    int rezultat = int.Parse(Encoding.UTF8.GetString(buffer, 0, brBajtova));
                    Console.WriteLine($"Igrac {igracTrenutni.Id} je bacio: {rezultat}");

                    string odgovorServera = "";

                    // Aktiviranje figure
                    if (!igracTrenutni.Figura.Aktivna)
                    {
                        if (rezultat == 6)      // Ako je dobijena 6 aktiviram figuru
                        {
                            igracTrenutni.Figura.Aktivna = true;
                            igracTrenutni.Figura.TrenutnaPozicija = igracTrenutni.Start;
                            igracTrenutni.Figura.UdaljenostDoCilja = poljaTable;
                            odgovorServera = "Figura je aktivirana! Dobili ste dodatni potez jer ste dobili 6.";
                        }
                        else                  
                        {
                            odgovorServera = "Morate baciti 6 da biste izasli! Gubite potez.";
                            igra.SledeciIgrac();
                        }
                    }
                    else // Figura je aktivna
                    {
                        // Proverava se cilj
                        if (igracTrenutni.Figura.UdaljenostDoCilja < rezultat)
                        {
                            odgovorServera = $"Ne mozete da udjete u cilj. Potreban tacan broj. Trenutno vam je potrebno: {igracTrenutni.Figura.UdaljenostDoCilja}.";
                            igra.SledeciIgrac();
                        }
                        else
                        {
                            igracTrenutni.Figura.Pomeraj(rezultat);
                            // Proveravamo da li je doslo do preklapanja figura 4. Zadatak
                            bool figuraPreklopljena = false;

                            foreach (var drugiIgrac in igra.Igraci.Where(i => i.Id != igracTrenutni.Id))
                            {
                                if (drugiIgrac.Figura.Aktivna && drugiIgrac.Figura.TrenutnaPozicija == igracTrenutni.Figura.TrenutnaPozicija)
                                {
                                    drugiIgrac.Figura.Aktivna = false;
                                    drugiIgrac.Figura.TrenutnaPozicija = -1;        // Vracamo ga na pocetak
                                    odgovorServera = $"Igrac {igracTrenutni.Id} je preklopio figuru igraca {drugiIgrac.Id}, koji je vracen u bazu!";
                                    figuraPreklopljena = true;
                                    break;
                                }
                            }
                            if (!figuraPreklopljena)
                            {
                                odgovorServera = $"Figura je pomerena. Ostalo Vam je do cilja: {igracTrenutni.Figura.UdaljenostDoCilja}";
                            }
                            if (rezultat != 6)
                            {
                                igra.SledeciIgrac();        // Prelazak na sledeceg igraca
                            }
                        }
                    }

                    // Proveravamo kraj igre
                    if (igracTrenutni.Figura.UdaljenostDoCilja == 0)
                    {
                        igra.KrajIgre = true;
                        string porukaPobeda = $"Igrac {igracTrenutni.Id} je pobedio! Kraj igre.";
                        foreach (var client in clients)
                        {
                            client.Send(Encoding.UTF8.GetBytes(porukaPobeda));
                        }
                        break;
                    }

                    // Poruka sa odgovorom
                    foreach (var client in clients)
                    {
                        client.Send(Encoding.UTF8.GetBytes(odgovorServera));
                    }

                    // 8. Izvestaj svim klijentima
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(" --- STANJE IGRE --- ");
                    foreach (var igrac in igra.Igraci)
                    {
                        sb.AppendLine($"Igrac: {igrac.Id}");
                        sb.AppendLine($"Aktivna figura: {igrac.Figura.Aktivna}");
                        sb.AppendLine($"Pozicija: {igrac.Figura.TrenutnaPozicija}");
                        sb.AppendLine($"Udaljenost do cilja: {igrac.Figura.UdaljenostDoCilja}");
                    }
                    string izvestaj = sb.ToString();
                    foreach (var client in clients)
                    {
                        client.Send(Encoding.UTF8.GetBytes(izvestaj));
                    }
                }
            }
            catch (Exception ex)
            {
                // Isprintaj gresku da znas sta se desilo
                Console.WriteLine($"Kriticna greska na serveru: {ex.Message}");
                Console.WriteLine($"Detalji: {ex.StackTrace}");
            }
            finally
            {
                Console.WriteLine("Server zavrsava sa radom.");
                foreach (var client in clients)
                {
                    client.Close();
                }
                serverSocket.Close();
                Console.ReadKey();
            }
        }
    }
}