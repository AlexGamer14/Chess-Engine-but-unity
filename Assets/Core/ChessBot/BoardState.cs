using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChessEngine
{
    public struct BoardState
    {
        // Piece positions
        public ulong WhitePawns;
        public ulong WhiteKnights;
        public ulong WhiteBishops;
        public ulong WhiteRooks;
        public ulong WhiteQueens;
        public ulong WhiteKing;

        public ulong BlackPawns;
        public ulong BlackKnights;
        public ulong BlackBishops;
        public ulong BlackRooks;
        public ulong BlackQueens;
        public ulong BlackKing;

        // Game state
        public ulong AllPieces;
        public ulong WhitePieces;
        public ulong BlackPieces;

        public bool WhiteToMove;

        public byte EnPassantTargetSquare;

        public bool WhiteCanCastleKingside;
        public bool WhiteCanCastleQueenside;
        public bool BlackCanCastleKingside;
        public bool BlackCanCastleQueenside;

        public float FiftyMoveRule;
        public float MoveCount;

        public BoardState SaveBoardState()
        {
            return new BoardState
            {
                WhitePawns = this.WhitePawns,
                WhiteKnights = this.WhiteKnights,
                WhiteBishops = this.WhiteBishops,
                WhiteRooks = this.WhiteRooks,
                WhiteQueens = this.WhiteQueens,
                WhiteKing = this.WhiteKing,

                BlackPawns = this.BlackPawns,
                BlackKnights = this.BlackKnights,
                BlackBishops = this.BlackBishops,
                BlackRooks = this.BlackRooks,
                BlackQueens = this.BlackQueens,
                BlackKing = this.BlackKing,

                AllPieces = this.AllPieces,
                WhitePieces = this.WhitePieces,
                BlackPieces = this.BlackPieces,

                WhiteToMove = this.WhiteToMove,

                EnPassantTargetSquare = this.EnPassantTargetSquare,

                WhiteCanCastleKingside = this.WhiteCanCastleKingside,
                WhiteCanCastleQueenside = this.WhiteCanCastleQueenside,
                BlackCanCastleKingside = this.BlackCanCastleKingside,
                BlackCanCastleQueenside = this.BlackCanCastleQueenside,

                FiftyMoveRule = this.FiftyMoveRule,
                MoveCount = this.MoveCount
            };
        }

        public void RestoreState(BoardState state)
        {
            WhitePawns = state.WhitePawns;
            WhiteKnights = state.WhiteKnights;
            WhiteBishops = state.WhiteBishops;
            WhiteRooks = state.WhiteRooks;
            WhiteQueens = state.WhiteQueens;
            WhiteKing = state.WhiteKing;

            BlackPawns = state.BlackPawns;
            BlackKnights = state.BlackKnights;
            BlackBishops = state.BlackBishops;
            BlackRooks = state.BlackRooks;
            BlackQueens = state.BlackQueens;
            BlackKing = state.BlackKing;

            AllPieces = state.AllPieces;
            WhitePieces = state.WhitePieces;
            BlackPieces = state.BlackPieces;

            WhiteToMove = state.WhiteToMove;

            EnPassantTargetSquare = state.EnPassantTargetSquare;

            WhiteCanCastleKingside = state.WhiteCanCastleKingside;
            WhiteCanCastleQueenside = state.WhiteCanCastleQueenside;
            BlackCanCastleKingside = state.BlackCanCastleKingside;
            BlackCanCastleQueenside = state.BlackCanCastleQueenside;

            FiftyMoveRule = state.FiftyMoveRule;
            MoveCount = state.MoveCount;
        }
    }

}