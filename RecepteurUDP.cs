using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace RecepteurReseau
{
    public class RecepteurUDP
    {
        private int port;
        private UdpClient socket;
        private IPEndPoint lastSenderEndPoint;
        private string saveFileName = "received_data.txt"; // Nom du fichier où les données seront sauvegardées.

        public RecepteurUDP(int port)
        {
            this.port = port;
            this.socket = new UdpClient(port);
        }

        public async Task DemarrerAsync()
        {
            Console.WriteLine("Attente de la connexion...");
            await AttendreSYNAsync();
            await RecevoirFichierAsync();
        }

        private async Task AttendreSYNAsync()
        {
            var packet = await socket.ReceiveAsync();
            lastSenderEndPoint = packet.RemoteEndPoint;
            var paquetRecu = new DecodagePaquet(packet.Buffer);

            if (paquetRecu.SynFlag && !paquetRecu.AckFlag)
            {
                await EnvoyerSYNACKAsync();
            }
        }

        private async Task EnvoyerSYNACKAsync()
        {
            var paquetSYNACK = new EncodagePaquet(1, true, true, false, false, new byte[0], lastSenderEndPoint.Address, lastSenderEndPoint.Port);
            var packetSYNACK = paquetSYNACK.CreationPaquet();
            await socket.SendAsync(packetSYNACK, lastSenderEndPoint);
            Console.WriteLine("Paquet SYN-ACK envoyé.");
        }

        private async Task RecevoirFichierAsync()
        {
            bool finReception = false;
            var receivedPackets = new List<byte[]>();

            while (!finReception)
            {
                var packet = await socket.ReceiveAsync();
                lastSenderEndPoint = packet.RemoteEndPoint; // Mise à jour de l'adresse du dernier expéditeur.
                var paquetRecu = new DecodagePaquet(packet.Buffer);

                if (paquetRecu.FinFlag)
                {
                    finReception = true;
                }
                else
                {
                    receivedPackets.Add(paquetRecu.Data);
                }
            }

            // Sauvegarde finale des données dans le répertoire de l'exécutable.
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), saveFileName);
            await File.WriteAllBytesAsync(fullPath, receivedPackets.SelectMany(a => a).ToArray());
            Console.WriteLine($"Fichier sauvegardé à : {fullPath}");

            await EnvoyerFINACKAsync();
        }

        private async Task EnvoyerFINACKAsync()
        {
            var paquetFINACK = new EncodagePaquet(1, false, true, true, false, new byte[0], lastSenderEndPoint.Address, lastSenderEndPoint.Port);
            var finAckPacket = paquetFINACK.CreationPaquet();
            await socket.SendAsync(finAckPacket, lastSenderEndPoint);
            Console.WriteLine("Paquet FIN-ACK envoyé.");
        }
    }
}
