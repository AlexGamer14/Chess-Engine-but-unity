using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChessEngine
{
    public static class HelperFunctions
    {
        public static byte RowAndColumnToByte(byte row, byte column)
        {
            return (byte)(row * 8 + column);
        }

        public static void SetBit(ref ulong bitboard, int bitPosition)
        {
            // Create and invert a mask where only the bit at bitPosition is 1
            ulong mask = ~(1UL << bitPosition);

            // Use bitwise AND to set the bit at the bitPosition
            bitboard &= mask;
        }

        public static void SetBit(ref ulong bitboard, int bitPosition, ulong desiredValue)
        {
            // Create a mask where only the bit at bitPosition is 1
            ulong mask = desiredValue << bitPosition;

            // Use bitwise AND to set the bit at the bitPosition
            bitboard |= mask;
        }


        public static byte GetBit(int byteIndex, ulong bytes)
        {
            return (bytes >> byteIndex) % 2 == 1 ? (byte)1 : (byte)0;
        }

        public static int CheckIfPieceOnEveryBoard(int position)
        {
            if (GetBit(position, ChessEngine.board.AllPieces) == 0) { return int.MaxValue; }

            if (GetBit(position, ChessEngine.board.WhitePawns) == 1)
            {
                return 0;
            }
            if (GetBit(position, ChessEngine.board.WhiteKnights) == 1)
            {
                return 1;
            }
            if (GetBit(position, ChessEngine.board.WhiteBishops) == 1)
            {
                return 2;
            }
            if (GetBit(position, ChessEngine.board.WhiteRooks) == 1)
            {
                return 3;
            }
            if (GetBit(position, ChessEngine.board.WhiteQueens) == 1)
            {
                return 4;
            }
            if (GetBit(position, ChessEngine.board.WhiteKing) == 1)
            {
                return 5;
            }

            if (GetBit(position, ChessEngine.board.BlackPawns) == 1)
            {
                return 6;
            }
            if (GetBit(position, ChessEngine.board.BlackKnights) == 1)
            {
                return 7;
            }
            if (GetBit(position, ChessEngine.board.BlackBishops) == 1)
            {
                return 8;
            }
            if (GetBit(position, ChessEngine.board.BlackRooks) == 1)
            {
                return 9;
            }
            if (GetBit(position, ChessEngine.board.BlackQueens) == 1)
            {
                return 10;
            }
            if (GetBit(position, ChessEngine.board.BlackKing) == 1)
            {
                return 11;
            }

            // failsafe
            return int.MaxValue;
        }
        public static int CheckIfPieceOnEveryBoard(int position, ChessBoard board)
        {
            if (GetBit(position, board.AllPieces)==0) { return int.MaxValue;  }

            if (GetBit(position, board.WhitePawns) == 1)
            {
                return 0;
            }
            if (GetBit(position, board.WhiteKnights) == 1)
            {
                return 1;
            }
            if (GetBit(position, board.WhiteBishops) == 1)
            {
                return 2;
            }
            if (GetBit(position, board.WhiteRooks) == 1)
            {
                return 3;
            }
            if (GetBit(position, board.WhiteQueens) == 1)
            {
                return 4;
            }
            if (GetBit(position, board.WhiteKing) == 1)
            {
                return 5;
            }

            if (GetBit(position, board.BlackPawns) == 1)
            {
                return 6;
            }
            if (GetBit(position, board.BlackKnights) == 1)
            {
                return 7;
            }
            if (GetBit(position, board.BlackBishops) == 1)
            {
                return 8;
            }
            if (GetBit(position, board.BlackRooks) == 1)
            {
                return 9;
            }
            if (GetBit(position, board.BlackQueens) == 1)
            {
                return 10;
            }
            if (GetBit(position, board.BlackKing) == 1)
            {
                return 11;
            }

            //failsafe
            return int.MaxValue;
        }

        public static byte GetBaseType(int byteIndex)
        {
            if (GetBit(byteIndex, ChessEngine.board.WhitePawns) == 1)
            {
                return 0;
            }
            if (GetBit(byteIndex, ChessEngine.board.BlackPawns) == 1)
            {
                return 6;
            }
            if (GetBit(byteIndex, ChessEngine.board.WhiteRooks) == 1)
            {
                return 3;
            }
            if (GetBit(byteIndex, ChessEngine.board.BlackRooks) == 1)
            {
                return 9;
            }
            if (GetBit(byteIndex, ChessEngine.board.WhiteKnights) == 1)
            {
                return 1;
            }
            if (GetBit(byteIndex, ChessEngine.board.BlackKnights) == 1)
            {
                return 7;
            }
            if (GetBit(byteIndex, ChessEngine.board.WhiteBishops) == 1)
            {
                return 2;
            }
            if (GetBit(byteIndex, ChessEngine.board.BlackBishops) == 1)
            {
                return 8;
            }
            if (GetBit(byteIndex, ChessEngine.board.WhiteQueens) == 1)
            {
                return 4;
            }
            if (GetBit(byteIndex, ChessEngine.board.BlackQueens) == 1)
            {
                return 10;
            }
            if (GetBit(byteIndex, ChessEngine.board.WhiteKing) == 1)
            {
                return 5;
            }
            if (GetBit(byteIndex, ChessEngine.board.BlackKing) == 1)
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

        public static ulong FlipBitboard(ulong bitboard)
        {
            ulong result = 0;
            for (int i = 0; i < 8; i++)
            {
                result |= ((bitboard >> (i * 8)) & 0xFF) << ((7 - i) * 8);
            }
            return result;
        }

        public static List<int> BitboardToList(ulong bitboard)
        {
            List<int> returnList = new List<int>();

            for (int i = 0; i < 64; i++)
            {
                returnList.Add(GetBit(i, bitboard));
            }

            return returnList;
        }

        public static void PrintList<T>(List<T> values)
        {
            PrintArray(values.ToArray());
        }

        public static void PrintArray<T>(T[] values) 
        {
            string str = "";

            foreach (T value in values)
            {
                str += value.ToString() + " ";
            }

            Debug.Log(str);
        }

        public static bool Contains<T>(T[] values, T value)
        {
            foreach (T val in values)
            {
                if (val.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}