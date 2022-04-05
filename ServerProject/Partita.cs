using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProject
{
    public class Partita
    {
        int ID1, ID2;
        string[] game;
        int[] caselleVincenti;
        bool fine;
        bool vittoria;
        int statoPartita;

        public int _ID1 { get => ID1; set { ID1 = value; } }

        public int _ID2 { get => ID2; set { ID2 = value; } }


        public bool Vinta { get => vittoria; }
        public bool Finita { get => fine; }
        public int StatoPartita { get => statoPartita; }
        public int[] CaselleVincenti { get => caselleVincenti; }


        public Partita()
        {
            ID1 = -2;
            ID1 = -1;
            game = new string[9];
            setMatrix();
            fine = false;
            vittoria = false;
            caselleVincenti = new int[3]; 
        }

        public Partita(int id1, int id2)
        {
            ID1 = id1;
            ID2 = id2;
            game = new string[9];
            setMatrix();
            fine = false;
            caselleVincenti = new int[3];
            vittoria = false;
        }

        public void showMatrix()
        {
            Console.WriteLine("\n");
            Console.Write(" ---------------");
            Console.WriteLine();

            int k = 0;
            for (int i=0; i<3; i++)
            {
                Console.Write("   |  ");
                for (int j = 0; j < 3; j++)
                    Console.Write(game[k++] + " ");
                Console.Write(" |");
                Console.WriteLine();
            }

            Console.Write(" ---------------");
            Console.WriteLine("\n");
        }

        private void setMatrix()
        {
            for (int i = 0; i < 9; i++) game[i] = "-";
        }

        public void mossa(int pos, string carattere)
        {
            game[pos] = carattere;
        }

        private void vincita(int pos1, int pos2, int pos3)
        {
            vittoria = true;
            fine = true;
            caselleVincenti[0] = pos1;
            caselleVincenti[1] = pos2;
            caselleVincenti[2] = pos3;
        }

        private void checkWin()
        {
            if (game[0] != "-" && game[0] == game[1] && game[0] == game[2])
                vincita(0, 1, 2);

            if (game[3] != "-" && game[3] == game[4] && game[3] == game[5])
                vincita(3, 4, 5);

            if (game[6] != "-" && game[6] == game[7] && game[6] == game[8])
                vincita(6, 7, 8);

            if (game[0] != "-" && game[0] == game[3] && game[0] == game[6])
                vincita(0, 3, 6);

            if (game[1] != "-" && game[1] == game[4] && game[1] == game[7])
                vincita(1, 4, 7);

            if (game[2] != "-" && game[2] == game[5] && game[2] == game[8])
                vincita(2, 5, 8);

            if (game[0] != "-" && game[0] == game[4] && game[0] == game[8])
                vincita(0, 4, 8);

            if (game[2] != "-" && game[2] == game[4] && game[2] == game[6])
                vincita(2, 4, 6);
        }

        public int[] getCaselleVincenti()
        {
            if (vittoria)
                return caselleVincenti;
            return null;
        }

        public string getVincitore()
        {
            if (vittoria)
                return game[caselleVincenti[0]];
            else return null;
        }

        public int getIdVincitore()
        {
            if (vittoria)
                return game[caselleVincenti[0]] == "X" ? ID1 : ID2;
            else return -1;
        }

        private void checkPartita()
        {
            checkPareggio();
            checkWin();
        }

        private void checkPareggio()
        {
            if (!game.Contains("-")) fine = true;
        }

        // 1 => partita vinta, 2 => pareggio, 0 => partita in corso
        public void aggiorna()
        {
            checkPartita();

            if (vittoria)
                statoPartita = 1;
            else
            {
                if (fine)
                    statoPartita = 2;
                else statoPartita = 0;
            }
        }
    }
}
