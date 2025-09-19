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
        //private static int trenutniIgracIndex = 0;
        private static int poljaTable = 20;
        private static Igra igra;
        static void Main(string[] args)
        {
            Console.WriteLine("Unesite broj igraca: ");
            int brojIgraca = int.Parse(Console.ReadLine());

            //Igraci i njihove figure
            List<Korisnik> igraci = new List<Korisnik>();

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 50001);
            serverSocket.Bind(serverEP);
            serverSocket.Listen(brojIgraca);

            Console.WriteLine("Server ceka i slusa igrace.");
            //Socket acceptedSocket = serverSocket.Accept(); zakom

            for (int i = 0; i < brojIgraca; i++)
            {
                Socket acceptedSocket = serverSocket.Accept();
                clients.Add(acceptedSocket);
                IPEndPoint clientEP = acceptedSocket.RemoteEndPoint as IPEndPoint;
                Console.WriteLine($"Igrac {i+1} je povezan: {clientEP}");
                

                // Kreiranje figure za korisnika
                Korisnik igrac = new Korisnik(i+1, "Igrac" + (i+1), 0, poljaTable);
                igraci.Add(igrac);
            }

            // Inicijalizacija igre
            igra = new Igra(igraci);
            Console.WriteLine("Svi igraci su povezani, POCINJEMO.");

            while (!igra.KrajIgre)      //bilo true
            {
                //Socket trenutniIgrac = clients[trenutniIgracIndex];
                //byte[] poruka = Encoding.UTF8.GetBytes("Vas je red! Bacite kockicu, pritisnite enter.\n");
                //trenutniIgrac.Send(poruka);
                try
                {
                    Korisnik igracTrenutni = igra.VratiTrenutnogIgraca(); //
                    Socket trenutniSocket = clients[igra.TrenutniIgrac];   //clients[trenutniIgracIndex];
                    
                    //Obavestavamo igraca da je on na redu
                    byte[] poruka = Encoding.UTF8.GetBytes("Vas je red! Bacite kockicu.");
                    trenutniSocket.Send(poruka);
                    
                    //Primam odgovor igraca
                    byte [] buffer = new byte[1024];
                    int brBajtova = trenutniSocket.Receive(buffer);
                    int rezultat = int.Parse(Encoding.UTF8.GetString(buffer, 0, brBajtova));
                    Console.WriteLine($"Igrac {igracTrenutni.Id} je bacio: {rezultat}");

                    //igracTrenutni.Figura.Pomeraj(rezultatBacanja);
                    //igra.ProveriZavrsetakIgre();

                    /*foreach (var client in clients)
                    {
                        if (client != trenutniIgrac)
                        {

                            client.Send(Encoding.UTF8.GetBytes($"Igrac {igracTrenutni.Id} je odigrao: {rezultatBacanja}"));
                            //trenutniIgrac.Send(Encoding.UTF8.GetBytes($"Igrac {igracTrenutni.Id} je odigrao: {rezultatBacanja}"));

                        }
                    }*/

                    //Aktivacija figure
                    if (!igracTrenutni.Figura.Aktivna)
                    {
                        if (rezultat == 6)
                        {
                            igracTrenutni.Figura.Aktivna = true;
                            igracTrenutni.Figura.TrenutnaPozicija = igracTrenutni.Start;
                            igracTrenutni.Figura.UdaljenostDoCilja = poljaTable;
                            trenutniSocket.Send(Encoding.UTF8.GetBytes("Figura je aktivirana!"));
                        }
                        else
                        {
                            trenutniSocket.Send(Encoding.UTF8.GetBytes("Morate baciti 6 da biste izasli!"));
                        }
                    }
                    else
                    {
                        igracTrenutni.Figura.Pomeraj(rezultat);
                        trenutniSocket.Send(Encoding.UTF8.GetBytes($"Figura pomerena. Ostalo do cilja: {igracTrenutni.Figura.UdaljenostDoCilja}"));

                        // Obavesti druge igrace
                        foreach (var client in clients)
                        {
                            if (client != trenutniSocket)
                            {
                                client.Send(Encoding.UTF8.GetBytes($"Igrac {igracTrenutni.Id} je pomerio figuru za {rezultat}"));
                            }
                        }
                    }

                    // Proveravamo kraj igre
                    if (igracTrenutni.Figura.UdaljenostDoCilja == 0)
                    {
                        igra.KrajIgre = true;
                        foreach (var client in clients)
                        {
                            client.Send(Encoding.UTF8.GetBytes($"Igrac {igracTrenutni.Id} je pobedio! Kraj igre."));
                        }
                        break;
                    }

                    igra.SledeciIgrac();
                    //trenutniIgracIndex = (trenutniIgracIndex + 1) % clients.Count;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Doslo je do greske {ex}");
                    break;
                }
            }


            Console.WriteLine("Server zavrsava sa radom");
            foreach (var client in clients)
            {
                client.Close();
            }
            //Console.ReadKey(); ovo ne znam da li je potrebno
            //acceptedSocket.Close();
            serverSocket.Close();
            
        }
    }
}


