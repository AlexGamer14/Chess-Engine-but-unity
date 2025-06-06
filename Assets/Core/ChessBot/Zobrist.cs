using System;

namespace ChessEngine
{
    public static class Zobrist
    {
        public static readonly ulong[,,] PieceSquareTable = new ulong[2, 6, 64]; // [color, piece, square]
        public static readonly ulong[] CastlingRights = new ulong[16];          // 4 bits
        public static readonly ulong[] EnPassantFile = new ulong[8];           // file a-h
        public static readonly ulong SideToMove;

        static Zobrist()
        {
            Random rng = new Random(123456); // seed for consistency

            for (int color = 0; color < 2; color++)
            {
                for (int piece = 0; piece < 6; piece++)
                {
                    for (int square = 0; square < 64; square++)
                    {
                        PieceSquareTable[color, piece, square] = RandomUlong(rng);
                    }
                }
            }

            for (int i = 0; i < 16; i++)
            {
                CastlingRights[i] = RandomUlong(rng);
            }

            for (int i = 0; i < 8; i++)
            {
                EnPassantFile[i] = RandomUlong(rng);
            }

            SideToMove = RandomUlong(rng);
        }

        private static ulong RandomUlong(Random rng)
        {
            byte[] buffer = new byte[8];
            rng.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}