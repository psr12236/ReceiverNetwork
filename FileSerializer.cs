using System;
using System.IO;
using System.Threading.Tasks;

namespace RecepteurReseau
{
    public class FileSerializer
    {
        public string NomFichier { get; private set; }
        public long TailleFichier { get; private set; }
        public byte[] Contenu { get; private set; }

        public async Task FileMetadataAsync(string cheminDocument)
        {
            if (!File.Exists(cheminDocument))
            {
                throw new FileNotFoundException("Le fichier spécifié n'existe pas", cheminDocument);
            }

            Contenu = await File.ReadAllBytesAsync(cheminDocument);
            NomFichier = Path.GetFileName(cheminDocument);
            TailleFichier = Contenu.Length;
        }

        public byte[] DonneeAEnvoyer()
        {
            // Assume that file name is encoded in UTF-8
            byte[] nomFichierBytes = System.Text.Encoding.UTF8.GetBytes(NomFichier);
            byte[] buffer = new byte[4 + nomFichierBytes.Length + Contenu.Length];

            using (var ms = new MemoryStream(buffer))
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(nomFichierBytes.Length);
                writer.Write(nomFichierBytes);
                writer.Write(Contenu);
            }

            return buffer;
        }

        public async Task SauvegarderFichierAsync(string cheminDestination)
        {
            string cheminComplet = Path.Combine(cheminDestination, NomFichier);

            // Create directory if it does not exist
            string directory = Path.GetDirectoryName(cheminComplet);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Save file
            await File.WriteAllBytesAsync(cheminComplet, Contenu);
            Console.WriteLine($"Fichier {NomFichier} sauvegardé sous {cheminComplet}.");
        }
    }
}
