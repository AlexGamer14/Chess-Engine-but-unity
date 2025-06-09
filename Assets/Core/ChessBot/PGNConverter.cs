using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChessEngine {
    public static class PGNConverter
    {
        public static string IndexToSquare(byte index)
        {
            char file = (char)('a' + (index % 8));
            int rank = 1 + (index / 8);
            return $"{file}{rank}";
        }

        public static string PromoteToChar(MovePieces.SpecialFlags promotion)
        {
            return promotion switch
            {
                MovePieces.SpecialFlags.Queen => "q",
                MovePieces.SpecialFlags.Rook => "r",
                MovePieces.SpecialFlags.Bishop => "b",
                MovePieces.SpecialFlags.Knight => "n",
                _ => ""
            };
        }

        public static string MoveToPGN(MovePieces.Move move)
        {
            string from = IndexToSquare(move.startPos);
            string to = IndexToSquare(move.endPos);
            string promotionChar = PromoteToChar(move.specialFlags);

            return $"{from}{to}{promotionChar}";
        }
    }
}