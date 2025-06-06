using System;
using UnityEngine;

namespace ChessEngine
{
    public class ChessBoard : ICloneable
    {
        // Bitboards for each piece type
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
        public ulong BlackKing ;
        //Casteling rights
        public byte CanCastle;

        public bool WhiteCanCastleQueenside;
        public bool WhiteCanCastleKingside;
        public bool BlackCanCastleQueenside;
        public bool BlackCanCastleKingside;

        // Bitboard for all pieces
        public ulong AllPieces;

        // Bitboard for all white pieces
        public ulong WhitePieces;

        // Bitboard for all black pieces
        public ulong BlackPieces;

        // En passant target square
        public byte EnPassantTargetSquare;
        
        // Attack boards
        public bool[] WhiteAttackBoard;
        public bool[] BlackAttackBoard;

        public bool WhiteToMove;

        public float FiftyMoveRule;

        public float MoveCount;


        // Initialize the board with the starting position
        public ChessBoard()
        {
            // Initialize the bitboards
            WhitePawns = 0x000000000000FF00; // Rank 2
            WhiteKnights = 0x0000000000000042; // b1 and g1
            WhiteBishops = 0x0000000000000024; // c1 and f1
            WhiteRooks = 0x0000000000000081; // a1 and h1
            WhiteQueens = 0x0000000000000008; // d1
            WhiteKing = 0x0000000000000010; // e1

            BlackPawns = 0x00FF000000000000; // Rank 7
            BlackKnights = 0x4200000000000000; // b8 and g8
            BlackBishops = 0x2400000000000000; // c8 and f8
            BlackRooks = 0x8100000000000000; // a8 and h8
            BlackQueens = 0x0800000000000000; // d8
            BlackKing = 0x1000000000000000; // e8

            // Calculate the bitboards for all pieces, white pieces, black pieces, and empty squares
            AllPieces = WhitePawns | WhiteKnights | WhiteBishops | WhiteRooks | WhiteQueens | WhiteKing |
                        BlackPawns | BlackKnights | BlackBishops | BlackRooks | BlackQueens | BlackKing;

            WhitePieces = WhitePawns | WhiteKnights | WhiteBishops | WhiteRooks | WhiteQueens | WhiteKing;
            BlackPieces = BlackPawns | BlackKnights | BlackBishops | BlackRooks | BlackQueens | BlackKing;

            EnPassantTargetSquare = byte.MaxValue;

            WhiteAttackBoard = new bool[64];
            BlackAttackBoard = new bool[64];
            
            WhiteCanCastleKingside = false;
            WhiteCanCastleQueenside = false;
            BlackCanCastleKingside = false;
            BlackCanCastleQueenside = false;

            WhiteToMove = true;

            FiftyMoveRule = 0;
            MoveCount = 0;
            
        }

        public void PrintBoard()
        {
            string boardText = "";

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    boardText += HelperFunctions.GetByte(j + i * 8, WhitePieces);
                }
                boardText += "\n";
            }

            Debug.Log(boardText);
        }

        public bool IsWhiteChecked()
        {
            for (byte x = 0; x < 64; x++)
            {
                if (HelperFunctions.GetByte(x, WhiteKing) == 1 && BlackAttackBoard[x])
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsWhiteStaleMate()
        {
            if (!IsWhiteChecked())
            {
                MovePieces.Move[] whiteMoves = MovePieces.GetMovesForBlackOrWhite(true, this);

                if (whiteMoves.Length == 0)
                {
                    // No legal moves available for white, but not in check
                    return true;
                }
                return false;
            }

            return false;
        }

        public bool IsWhiteCheckMate()
        {
            if (IsWhiteChecked())
            {
                MovePieces.Move[] whiteMoves = MovePieces.GetMovesForBlackOrWhite(true, this);

                if (whiteMoves.Length == 0)
                {
                    // No legal moves available for white, but not in check
                    return true;
                }
                return false;
            }

            return false;
        }

        public bool IsBlackStaleMate()
        {
            if (!IsBlackChecked())
            {
                MovePieces.Move[] blackMoves = MovePieces.GetMovesForBlackOrWhite(false, this);

                if (blackMoves.Length == 0)
                {
                    // No legal moves available for white, but not in check
                    return true;
                }
                return false;
            }

            return false;
        }

        public bool IsBlackCheckMate()
        {
            if (IsBlackChecked())
            {
                MovePieces.Move[] blackMoves = MovePieces.GetMovesForBlackOrWhite(false, this);

                if (blackMoves.Length == 0)
                {
                    // No legal moves available for white, but not in check
                    return true;
                }
                return false;
            }

            return false;
        }
        public bool IsBlackChecked()
        {
            for (byte x = 0; x < 64; x++)
            {
                if (HelperFunctions.GetByte(x, BlackKing) == 1 && WhiteAttackBoard[x])
                {
                    return true;
                }
            }

            return false;
        }

        public ulong ComputeZobristHash()
        {
            ulong hash = 0;

            // Pieces
            AddPieceHash(WhitePawns, 0, 0);
            AddPieceHash(WhiteBishops, 0, 2);
            AddPieceHash(WhiteKnights, 0, 1);
            AddPieceHash(WhiteRooks, 0, 3);
            AddPieceHash(WhiteQueens, 0, 4);
            AddPieceHash(WhiteKing, 0, 5);

            AddPieceHash(BlackPawns, 1, 0);
            AddPieceHash(BlackKnights, 1, 1);
            AddPieceHash(BlackBishops, 1, 2);
            AddPieceHash(BlackRooks, 1, 3);
            AddPieceHash(BlackQueens, 1, 4);
            AddPieceHash(BlackKing, 1, 5);

            // Castling rights: encode as 4 bits
            int castlingKey = 0;
            if (WhiteCanCastleKingside) castlingKey |= 1;
            if (WhiteCanCastleQueenside) castlingKey |= 2;
            if (BlackCanCastleKingside) castlingKey |= 4;
            if (BlackCanCastleQueenside) castlingKey |= 8;
            hash ^= Zobrist.CastlingRights[castlingKey];

            // En passant file (if valid)
            if (EnPassantTargetSquare < 64)
            {
                int file = (EnPassantTargetSquare % 8);
                hash ^= Zobrist.EnPassantFile[file];
            }

            // Side to move
            if (WhiteToMove)
                hash ^= Zobrist.SideToMove;

            return hash;

            // Local helper function
            void AddPieceHash(ulong bitboard, int color, int pieceType)
            {
                for (int square = 0; square < 64; square++)
                {
                    if (((bitboard >> square) & 1) != 0)
                    {
                        hash ^= Zobrist.PieceSquareTable[color, pieceType, square];
                    }
                }
            }
        }


        public void UpdateBitBoards()
        {
            WhitePieces = WhitePawns | WhiteKnights | WhiteBishops | WhiteRooks | WhiteQueens | WhiteKing;
            BlackPieces = BlackPawns | BlackKnights | BlackBishops | BlackRooks | BlackQueens | BlackKing;

            AllPieces = WhitePieces | BlackPieces;
        }

        public static ulong SetBit(ulong bitboard, int bitPosition, bool value)
        {
            if (bitPosition < 0 || bitPosition >= 64)
            {
                throw new ArgumentOutOfRangeException(nameof(bitPosition), "Bit position must be between 0 and 63.");
            }

            ulong mask = 1UL << bitPosition;

            if (value)
            {
                return bitboard | mask;
            }
            else
            {
                return bitboard & ~mask;
            }
        }

        public void ClearBoard()
        {
            this.WhiteBishops = 0;
            this.WhiteKing = 0;
            this.WhiteKnights = 0;
            this.WhitePawns = 0;
            this.WhiteQueens = 0;
            this.WhiteRooks = 0;

            this.BlackBishops = 0;
            this.BlackKing = 0;
            this.BlackKnights = 0;
            this.BlackPawns = 0;
            this.BlackQueens = 0;
            this.BlackRooks = 0;
        }

        public void SetUpAllBoards()
        {
            this.WhitePieces = this.WhitePawns | this.WhiteKnights | this.WhiteBishops | this.WhiteRooks | this.WhiteQueens | this.WhiteKing;
            this.BlackPieces = this.BlackPawns | this.BlackKnights | this.BlackBishops | this.BlackRooks | this.BlackQueens | this.BlackKing;
        }


        public bool Equals(ChessBoard board)
        {
            if (this.AllPieces != board.AllPieces)
            {
                Debug.Log("AllPieces are different.");
                return false;
            }
            if (this.WhitePawns != board.WhitePawns)
            {
                Debug.Log("WhitePawns are different.");
                return false;
            }
            if (this.WhiteKnights != board.WhiteKnights)
            {
                Debug.Log("WhiteKnights are different.");
                return false;
            }
            if (this.WhiteBishops != board.WhiteBishops)
            {
                Debug.Log("WhiteBishops are different.");
                return false;
            }
            if (this.WhiteRooks != board.WhiteRooks)
            {
                Debug.Log("WhiteRooks are different.");
                return false;
            }
            if (this.WhiteQueens != board.WhiteQueens)
            {
                Debug.Log("WhiteQueens are different.");
                return false;
            }
            if (this.WhiteKing != board.WhiteKing)
            {
                Debug.Log("WhiteKing is different.");
                return false;
            }

            if (this.BlackPawns != board.BlackPawns)
            {
                Debug.Log("BlackPawns are different.");
                return false;
            }
            if (this.BlackKnights != board.BlackKnights)
            {
                Debug.Log("BlackKnights are different.");
                return false;
            }
            if (this.BlackBishops != board.BlackBishops)
            {
                Debug.Log("BlackBishops are different.");
                return false;
            }
            if (this.BlackRooks != board.BlackRooks)
            {
                Debug.Log("BlackRooks are different.");
                return false;
            }
            if (this.BlackQueens != board.BlackQueens)
            {
                Debug.Log("BlackQueens are different.");
                return false;
            }
            if (this.BlackKing != board.BlackKing)
            {
                Debug.Log("BlackKing is different.");
                return false;
            }
            if (this.BlackAttackBoard != board.BlackAttackBoard)
            {
                Debug.Log("BlackAttackBoard are different.");
                return false;
            }
            if (this.WhiteAttackBoard != board.WhiteAttackBoard)
            {
                Debug.Log("WhiteAttackBoard are different.");
                return false;
            }

            return true;
        }


        public object Clone()
        {
            ChessBoard newBoard = new()
            {
                // Copy individual piece bitboards
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

                // Copy bitboards for all pieces
                WhitePieces = this.WhitePieces,
                BlackPieces = this.BlackPieces,
                AllPieces = this.AllPieces,

                EnPassantTargetSquare = this.EnPassantTargetSquare,

                BlackAttackBoard = this.BlackAttackBoard,
                WhiteAttackBoard = this.BlackAttackBoard,

                WhiteCanCastleKingside = this.WhiteCanCastleKingside,
                WhiteCanCastleQueenside = this.WhiteCanCastleQueenside,
                BlackCanCastleKingside = this.BlackCanCastleKingside,
                BlackCanCastleQueenside = this.BlackCanCastleQueenside,

                WhiteToMove = this.WhiteToMove,

                FiftyMoveRule = this.FiftyMoveRule,
                MoveCount = this.MoveCount,

            };

            return newBoard;
        }
    }
}
