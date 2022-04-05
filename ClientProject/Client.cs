using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientProject
{
    public class Client
    {
        Socket client;
        string nickname;
        string[] tris = new string[9];
        string myChar;
        string avversario;
        bool turno;

        public string Nickname { get => nickname; set => nickname = value; }
        public string Avversario { get => avversario; }
        public string My { get => myChar; }
        public bool Turno { get => turno; }

        public Client()
        {
            client = null;
            setMatrix();
            myChar = "NA";
            avversario = "NA";
            turno = false;
        }

        private void setMatrix()
        {
            for (int i = 0; i < 9; i++)
                tris[i] = "-";
        }

        public void setTurno()
        {
            if (myChar == "O") turno = false; else turno = true;
        }

        public void cambiaTurno()
        {
            turno = !turno;
        }

        public void invia(string messaggio)
        {
            byte[] bytes = new byte[1024];
            byte[] msg = Encoding.ASCII.GetBytes(messaggio);
            int bytesSent = client.Send(msg);
        }

        public void start()
        {
            Connect();
            attendoPartita();
        }

        public void Connect()
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 10011);

                try
                {
                    client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    client.Connect(remoteEP);
                    Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());
                    Console.WriteLine();

                    string stato = ricevi();
                    Console.WriteLine("Stato (1/2): " + stato);
                }                
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void attendoPartita()
        {
            string inizio = ricevi();
            Console.WriteLine("Inizio partita: " + inizio);
        }

        public void gestioneInformazioni()
        {
            Console.WriteLine("\nClient: invio al server il mio nickname: " + nickname);
            invia(nickname);

            string[] risposta = ricevi().Split(',');

            avversario = risposta[0];
            myChar = risposta[1];

            Console.WriteLine("Nickname avversario: " + avversario + ", mio carattere: " + myChar);
        }
            
        private void stop()
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        public void mossa(int index)
        {
            try
            {
                Console.WriteLine("Mia casella: " + index);
                invia(index.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Eccezione, dettagli: \n\n" + e + "\n -----------------------------------");
            }
        }

        public int riceviMossa()
        {
            try
            {
                int mossaAvversario = int.Parse(ricevi());
                Console.WriteLine("Casella avversaro: " + mossaAvversario);

                return mossaAvversario;
            }
            catch (Exception e)
            {
                Console.WriteLine("Eccezione, dettagli: \n\n" + e + "\n -----------------------------------");
                return -1;
            }
        }

        private void mostraMatrix()
        {
            Console.WriteLine("\n");

            int k = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    Console.Write(tris[k++]);
                Console.Write("\n");
            }

            Console.WriteLine("\n");
        }

        public string notMy()
        {
            return myChar == "X" ? "O" : "X";
        }

        public string ricevi()
        {
            try
            {
                byte[] bytes = new byte[1024];
                int bytesRec = client.Receive(bytes);
                return Encoding.ASCII.GetString(bytes, 0, bytesRec);
            }
            catch (Exception) 
            {
                Console.WriteLine("Eccezione generata");
                return "Eccezione";
            }
        }
    }
}
