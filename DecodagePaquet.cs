using System;
using System.Linq;

namespace RecepteurReseau
{
    public class DecodagePaquet
    {
        private const int HEADER_SIZE = 8;
        public short NumSequence { get; }
        public bool SynFlag { get; }
        public bool AckFlag { get; }
        public bool FinFlag { get; }
        public bool RstFlag { get; }
        public byte[] Data { get; }

        public DecodagePaquet(byte[] packetData)
        {
            if (packetData.Length < HEADER_SIZE)
            {
                throw new ArgumentException("Paquet reçu trop court pour contenir un en-tête valide.");
            }

            NumSequence = (short)((packetData[0] << 8) | packetData[1]);
            byte flags = packetData[2];
            SynFlag = (flags & 0b1000) != 0;
            AckFlag = (flags & 0b0100) != 0;
            FinFlag = (flags & 0b0010) != 0;
            RstFlag = (flags & 0b0001) != 0;

            // We assume here that the checksum is correct.
            Data = packetData.Skip(HEADER_SIZE).ToArray();
        }
    }
}
