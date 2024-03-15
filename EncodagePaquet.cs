using System;
using System.Net;
using System.Linq;

namespace RecepteurReseau
{
    public class EncodagePaquet
    {
        public static readonly int HEADER_SIZE = 8; // La taille de l'entête inclut le checksum.
        private short numSequence;
        private bool synFlag, ackFlag, finFlag, rstFlag;
        private byte[] data;
        private IPAddress destinationAddress;
        private int destinationPort;

        public EncodagePaquet(short numSequence, bool synFlag, bool ackFlag, bool finFlag, bool rstFlag, byte[] data, IPAddress address, int port)
        {
            this.numSequence = numSequence;
            this.synFlag = synFlag;
            this.ackFlag = ackFlag;
            this.finFlag = finFlag;
            this.rstFlag = rstFlag;
            this.data = data ?? throw new ArgumentNullException(nameof(data));
            this.destinationAddress = address ?? throw new ArgumentNullException(nameof(address));
            this.destinationPort = port;
        }

        public byte[] CreationPaquet()
        {
            byte[] packetData = new byte[HEADER_SIZE + data.Length];
            packetData[0] = (byte)(numSequence >> 8);
            packetData[1] = (byte)(numSequence);
            packetData[2] = (byte)((synFlag ? 0b1000 : 0) | (ackFlag ? 0b0100 : 0) | (finFlag ? 0b0010 : 0) | (rstFlag ? 0b0001 : 0));
            // Ici, le checksum est simplement simulé par deux octets à zéro.
            // Une vraie application devrait implémenter une fonction de calcul de checksum robuste.
            packetData[3] = 0; // Simulated checksum byte 1
            packetData[4] = 0; // Simulated checksum byte 2
            // Copy data
            Array.Copy(data, 0, packetData, HEADER_SIZE, data.Length);

            return packetData;
        }

        private short CalculateChecksum(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            // Implémentation simplifiée d'un checksum. À améliorer pour une utilisation réelle.
            int checksum = data.Aggregate(0, (sum, next) => sum + next);
            return (short)(checksum % short.MaxValue);
        }

        // Les getters sont convertis en propriétés en C#.
        public short NumSequence => numSequence;
        public bool SynFlag => synFlag;
        public bool AckFlag => ackFlag;
        public bool FinFlag => finFlag;
        public bool RstFlag => rstFlag;
        public byte[] Data => data;
        public IPAddress DestinationAddress => destinationAddress;
        public int DestinationPort => destinationPort;
    }
}
