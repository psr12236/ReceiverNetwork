using System;
using System.IO;
using System.Threading.Tasks;

namespace RecepteurReseau
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Le chemin de travail est le répertoire courant de l'exécutable.
            RecepteurUDP recepteur = new RecepteurUDP(9876);
            Console.WriteLine("Starting the UDP receiver...");
            await recepteur.DemarrerAsync();
            Console.WriteLine("The UDP receiver has completed receiving data.");
        }
    }
}
