using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TCPClient
{
    internal class Client
    {
        static void Main(string[] args)
        {

            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Loopback, 50001);

            Console.WriteLine("Klijent je spreman za povezivanje sa serverom...");
            Console.ReadKey();
         
            try
            {
                clientSocket.Connect(serverEP);
                Console.WriteLine("Povezani ste na server.");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Greška pri povezivanju sa serverom: {ex.Message}");
                Console.ReadKey();
                return;
            }

            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int brBajtova = clientSocket.Receive(buffer);
                    if (brBajtova == 0)
                    {
                        Console.WriteLine("Server je prekinuo konekciju.");
                        break;
                    }

                    string serverPoruka = Encoding.UTF8.GetString(buffer, 0, brBajtova);

                    if (serverPoruka.Contains("Vas je red!"))
                    {
                        Console.WriteLine("SERVER:" + serverPoruka);//ovo sam ja
                        Console.WriteLine("Pritisnite enter da biste bacili kockicu.");
                        Console.ReadKey();

                        Random rnd = new Random();
                        int rezultatKockice = rnd.Next(1, 7);
                        Console.WriteLine($"Vas rezultat je:{rezultatKockice}\n");

                        // Rezultat se salje serveru
                        clientSocket.Send(Encoding.UTF8.GetBytes(rezultatKockice.ToString()));

                        // Klient prima odgovor o ishodu poteza
                        brBajtova = clientSocket.Receive(buffer);
                        string konacnaPoruka = Encoding.UTF8.GetString(buffer, 0, brBajtova); 
                        
                        string[] deloviPoruke = konacnaPoruka.Split(new string[] { "---" }, StringSplitOptions.None);
  
                        if (deloviPoruke.Length > 1)
                        {
                            string odgovorServera = deloviPoruke[0];
                            string izvestaj = deloviPoruke[1];

                            Console.WriteLine(odgovorServera);
                            Console.WriteLine(izvestaj);
                        }
                        else
                        {
                            // Ako nema separatora, ispiši celu poruku
                            Console.WriteLine(konacnaPoruka);
                        }
                    } 
                    else
                    {
                        Console.WriteLine(serverPoruka);
                    }

                }
                catch (SocketException ex) 
                {
                    Console.WriteLine($"Doslo je do greske: {ex.Message}");
                    break;
                }
                
            }
            Console.WriteLine("Klijent zavrsava sa radom");
            Console.ReadKey();
            clientSocket.Close();

        }
    }
}

/*

            //byte[] buffer = new byte[1024];

            int bytesRead = clientSocket.Receive(buffer);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine(message);

            while (true)
            {
                if (message.StartsWith("Tvoj je potez."))
                {
                    Console.WriteLine("Unesite broj kockice:");
                    string potez = Console.ReadLine();
                    clientSocket.Send(Encoding.UTF8.GetBytes(potez));
                }
            }*/
