using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProject
{
    class Program
    {
        static void Main(string[] args)
        {
            new Server().startServer();
            Console.Write("Premere un tasto per uscire . . ."); Console.ReadLine();
        }
    }
}
