using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace ServerProject
{
    class Server
    {
        Socket server;
        List<Partita> partite;
        List<Socket> clients;

        public Server()
        {
            server = null;
            clients = new List<Socket>();
            partite = new List<Partita>();
        }

        private int getID()
        {
            return clients.Count - 1;
        }

        private string ricevi(int id)
        {
            byte[] bytes = null;
            bytes = new byte[1024];

            int bytesRec = clients[id].Receive(bytes);

            return Encoding.ASCII.GetString(bytes, 0, bytesRec);
        }

        private void invia(string testo, int id)
        {
            byte[] msg = Encoding.ASCII.GetBytes(testo);
            clients[id].Send(msg);
        }

        public void startServer()
        {
            try
            {
                Console.WriteLine("\n Server started, Main thread ID: {0}",Thread.CurrentThread.ManagedThreadId + "\n\n ______________________________________________________ \n\n\n");

                IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 10011);

                server = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(localEndPoint);
                server.Listen(50);

                while (true)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Console.WriteLine(" Thread MAIN: in attesa di connessioni...");
                        clients.Add(server.Accept());
                        Console.WriteLine(" Thread MAIN: client " + getID() + " connesso!");
                        if (i == 0) invia("Sei il primo, attendi un giocatore", getID()); else invia("Ti sei unito a una partita avviata", getID());
                    }

                    Thread.Sleep(500);
                    creaPartita();
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("ERROR! : un giocatore si e' disconnesso #" + getID() + "\n --------------------- \n" + error.Message);
            }
        }

        private void creaPartita()
        {
            for (int i = 0; i < 2; i++)
                invia("La partita sta per iniziare ... ", getID() - 1 + i);
            
            // Risorsa condivisa (tabella tris) tra i due thread che gestiscono la partita tra giocatore pari e dispari

            Partita partita = new Partita();
            partite.Add(partita);

            for (int i = 0; i < 2; i++)
            {
                Thread thr = new Thread(new ThreadStart(() => gestisci_inizioPartita(getID() - 1 + i, partita)));
                thr.Start();
                Thread.Sleep(500);
            }
        }

        private void gestisci_chat(int id)
        {
            int idAvversario = id % 2 == 0 ? id + 1 : id - 1;
            while (true)
            {
                string risposta = ricevi(id);
                invia(risposta, idAvversario);
            }
        }

        /*
        private string gestisciNickname(int id)
        {
            while (true)
            {
                string nickname = ricevi(id);
                if (nicknames.Contains(nickname))
                {
                    invia("nickname gia' utilizzato", id);
                    continue;
                }
                invia("nickname OK", id);
                return nickname;
            }
        }*/

        private void gestisci_inizioPartita(int id, Partita partita)
        {
            string idthr = " Thread ID-" + Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine(idthr+ ": gestisco messaggi provenienti da client " + id);
            try
            {
                //string nickname = gestisciNickname(id);
                string nickname = ricevi(id);

                Console.WriteLine(idthr+": ho ricevuto " + nickname + " da client " + id);

                int idAvversario = id % 2 == 0 ? id + 1 : id - 1;

                if (id % 2 == 0)
                    partita._ID1 = id;
                else
                    partita._ID2 = id;

                // invio nickname avversario e il carattere che utilizzerà X oppure O
                string risposta = nickname + "," + getChar(idAvversario);

                Console.WriteLine(idthr+": invio al client " + idAvversario + " il seguente messaggio: " + risposta);

                Thread.Sleep(1000);

                invia(risposta, idAvversario);

                gestione_partita(id, idAvversario, partita);
            }
            catch (Exception E)
            {
                Console.WriteLine(idthr+": ECCEZIONE, \n\n"+E);
                //clients.RemoveAt(id);
            }

            //mossa(id);
        }

        private string getChar(int id)
        {
            return id % 2 == 0 ? "X" : "O";
        }

        private void gestione_partita(int id, int idAvversario, Partita partita)
        {
            string idthr = " Thread ID-" + Thread.CurrentThread.ManagedThreadId;
            while (true)
            {
                int pos = int.Parse(ricevi(id));

                if (pos == -1) { break; }

                Console.WriteLine(idthr+": ho ricevuto casella n." + pos + " da client " + id);

                partita.mossa(pos, getChar(id));
                partita.aggiorna();

                string risposta = pos.ToString();

                if (partita.Finita)
                {
                    int idW = gestione_finePartita(risposta, partita, id, idAvversario);
                    if (idW == -10)
                         Console.WriteLine("\n" + idthr + ": Pareggio \n - tabella finale =>");
                    else Console.WriteLine("\n" + idthr + ": La partita è terminata, ha vinto il giocatore ID-"+idW+" ["+getChar(idW)+"]\n - tabella finale =>"); 
                    partita.showMatrix();

                    partite.Remove(partita);

                    Socket[] sockets = getSockets(id, idAvversario);
                    chiudiConnessione(sockets[0], id);
                    chiudiConnessione(sockets[1], idAvversario);

                    break;

                } else risposta += ",IN CORSO";

                partita.showMatrix();

                Console.WriteLine(idthr+": invio al client " + idAvversario + " il seguente messaggio: " + risposta);

                invia(risposta, idAvversario);

                Thread.Sleep(600);
            }
        }
        /*
        private void gestisci_disconnessione(int id)
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId+": ciao");
            string chiusura = ricevi(id);
            if (chiusura == "CLOSING")
            {
                clients.RemoveAt(id);
                Console.WriteLine(chiusura);
            }

            Console.WriteLine("Giocatore ID {0} disconnesso", id);
        }*/

        private void chiudiConnessione(Socket s, int id)
        {
            clients.Remove(s);
            Console.WriteLine(" Thread "+Thread.CurrentThread.ManagedThreadId + ": client ID-{0} disconnesso",id);
        }

        private Socket[] getSockets(int id1, int id2)
        {
            Socket[] s_array = new Socket[2];
            s_array[0] = clients[id1];
            s_array[1] = clients[id2];

            return s_array;
        }

        private int gestione_finePartita(string ultimaMossa, Partita partita, int id, int idAvversario)
        {
            string rispostaChiamante = ultimaMossa;
            string rispostaAscolto = ultimaMossa;
            if (partita.Vinta)
            {
                int[] CV = partita.getCaselleVincenti();
                string vincenti = CV[0].ToString() + "_" + CV[1].ToString() + "_" + CV[2].ToString();

                rispostaChiamante += ",VINTO," + vincenti;
                rispostaAscolto += ",PERSO," + vincenti;
                invia(rispostaChiamante, id);
                invia(rispostaAscolto, idAvversario);
                return partita.getIdVincitore();
            }
            else
            {
                rispostaChiamante += ",PAREGGIO";
                invia(rispostaChiamante, id);
                invia(rispostaChiamante, idAvversario);
                return -10;
            }
        }
    }
}
