using System;

namespace ChessEngine
{
    public static class HelperFunctions
    {
        public static byte RowAndColumnToByte(byte row, byte column)
        {
            return (byte)(row * 8 + column);
        }

        public static void SetBit(ref ulong value, int bitPosition)
        {
            // Create a mask where only the bit at bitPosition is 1
            ulong mask = ~(1UL << bitPosition);

            // Use bitwise AND to clear the bit at the bitPosition
            value &= mask;
        }


        public static byte GetByte(int byteIndex, ulong bytes)
        {
            return (bytes >> byteIndex) % 2 == 1 ? (byte)1 : (byte)0;
        }

        public static int CheckIfPieceOnEveryBoard(int pieceToNotCheck, int position)
        {
            if (pieceToNotCheck != 0 && GetByte(position, ChessEngine.board.WhitePawns) == 1)
            {
                return 0;
            }
            if (pieceToNotCheck != 1 && GetByte(position, ChessEngine.board.WhiteKnights) == 1)
            {
                return 1;
            }
            if (pieceToNotCheck != 2 && GetByte(position, ChessEngine.board.WhiteBishops) == 1)
            {
                return 2;
            }
            if (pieceToNotCheck != 3 && GetByte(position, ChessEngine.board.WhiteRooks) == 1)
            {
                return 3;
            }
            if (pieceToNotCheck != 4 && GetByte(position, ChessEngine.board.WhiteQueens) == 1)
            {
                return 4;
            }
            if (pieceToNotCheck != 5 && GetByte(position, ChessEngine.board.WhiteKing) == 1)
            {
                return 5;
            }

            if (pieceToNotCheck != 6 && GetByte(position, ChessEngine.board.BlackPawns) == 1)
            {
                return 6;
            }
            if (pieceToNotCheck != 7 && GetByte(position, ChessEngine.board.BlackKnights) == 1)
            {
                return 7;
            }
            if (pieceToNotCheck != 8 && GetByte(position, ChessEngine.board.BlackBishops) == 1)
            {
                return 8;
            }
            if (pieceToNotCheck != 9 && GetByte(position, ChessEngine.board.BlackRooks) == 1)
            {
                return 9;
            }
            if (pieceToNotCheck != 10 && GetByte(position, ChessEngine.board.BlackQueens) == 1)
            {
                return 10;
            }
            if (pieceToNotCheck != 11 && GetByte(position, ChessEngine.board.BlackKing) == 1)
            {
                return 11;
            }

            //fail  safe
            return int.MaxValue;
        }
        public static int CheckIfPieceOnEveryBoard(int pieceToNotCheck, int position, ChessBoard board)
        {
            if (pieceToNotCheck != 0 && GetByte(position, board.WhitePawns) == 1)
            {
                return 0;
            }
            if (pieceToNotCheck != 1 && GetByte(position, board.WhiteKnights) == 1)
            {
                return 1;
            }
            if (pieceToNotCheck != 2 && GetByte(position, board.WhiteBishops) == 1)
            {
                return 2;
            }
            if (pieceToNotCheck != 3 && GetByte(position, board.WhiteRooks) == 1)
            {
                return 3;
            }
            if (pieceToNotCheck != 4 && GetByte(position, board.WhiteQueens) == 1)
            {
                return 4;
            }
            if (pieceToNotCheck != 5 && GetByte(position, board.WhiteKing) == 1)
            {
                return 5;
            }

            if (pieceToNotCheck != 6 && GetByte(position, board.BlackPawns) == 1)
            {
                return 6;
            }
            if (pieceToNotCheck != 7 && GetByte(position,   board.BlackKnights) == 1)
            {
                return 7;
            }
            if (pieceToNotCheck != 8 && GetByte(position,   board.BlackBishops) == 1)
            {
                return 8;
            }
            if (pieceToNotCheck != 9 && GetByte(position, board.BlackRooks) == 1)
            {
                return 9;
            }
            if (pieceToNotCheck != 10 && GetByte(position, board.BlackQueens) == 1)
            {
                return 10;
            }
            if (pieceToNotCheck != 11 && GetByte(position, board.BlackKing) == 1)
            {
                return 11;
            }

            //fail  safe
            return int.MaxValue;
        }

        public static byte GetBaseType(int byteIndex)
        {
            if (GetByte(byteIndex, ChessEngine.board.WhitePawns) == 1)
            {
                return 0;
            }
            if (GetByte(byteIndex, ChessEngine.board.BlackPawns) == 1)
            {
                return 6;
            }
            if (GetByte(byteIndex, ChessEngine.board.WhiteRooks) == 1)
            {
                return 3;
            }
            if (GetByte(byteIndex, ChessEngine.board.BlackRooks) == 1)
            {
                return 9;
            }
            if (GetByte(byteIndex, ChessEngine.board.WhiteKnights) == 1)
            {
                return 1;
            }
            if (GetByte(byteIndex, ChessEngine.board.BlackKnights )== 1)
            {
                return 7;
            }
            if (GetByte(byteIndex, ChessEngine.board.WhiteBishops) == 1)
            {
                return 2;
            }
            if (GetByte(byteIndex, ChessEngine.board.BlackBishops) == 1)
            {
                return 8;
            }
            if (GetByte(byteIndex, ChessEngine.board.WhiteQueens) == 1)
            {
                return 4;
            }
            if (GetByte(byteIndex, ChessEngine.board.BlackQueens) == 1)
            {
                return 10;
            }
            if (GetByte(byteIndex, ChessEngine.board.WhiteKing) == 1)
            {
                return 5;
            }
            if (GetByte(byteIndex, ChessEngine.board.BlackKing) == 1)
            {
                return 11;
            }

            return byte.MaxValue;
        }

        public static ref ulong GetTypeBasedOnIndex(int index)
        {
                switch (index)
                {
                    case 0:
                        return ref ChessEngine.board.WhitePawns;
                    case 1:
                        return ref ChessEngine.board.WhiteKnights;
                    case 2:
                        return ref ChessEngine.board.WhiteBishops;
                    case 3:
                        return ref ChessEngine.board.WhiteRooks;
                    case 4:
                        return ref ChessEngine.board.WhiteQueens;
                    case 5:
                        return ref ChessEngine.board.WhiteKing;
                    case 6:
                        return ref ChessEngine.board.BlackPawns;
                    case 7:
                        return ref ChessEngine.board.BlackKnights;
                    case 8:
                        return ref ChessEngine.board.BlackBishops;
                    case 9:
                        return ref ChessEngine.board.BlackRooks;
                    case 10:
                        return ref ChessEngine.board.BlackQueens;
                    case 11:
                        return ref ChessEngine.board.BlackKing;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Invalid piece index.");
                }
        }
        public static ref ulong GetTypeBasedOnIndex(int index, ref ChessBoard board)
        {
            switch (index)
            {
                case 0:
                    return ref board.WhitePawns;
                case 1:
                    return ref board.WhiteKnights;
                case 2:
                    return ref board.WhiteBishops;
                case 3:
                    return ref board.WhiteRooks;
                case 4:
                    return ref board.WhiteQueens;
                case 5:
                    return ref board.WhiteKing;
                case 6:
                    return ref board.BlackPawns;
                case 7:
                    return ref board.BlackKnights;
                case 8:
                    return ref board.BlackBishops;
                case 9:
                    return ref board.BlackRooks;
                case 10:
                    return ref board.BlackQueens;
                case 11:
                    return ref board.BlackKing;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), "Invalid piece index. " + index);
            }
        }
    }
}