using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

namespace ChessEngine
{
    public static class MovePieces
    {
        public static System.Diagnostics.Stopwatch moveGenTime = new System.Diagnostics.Stopwatch();

        public enum SpecialFlags
        {
            Knight,
            Bishop,
            Rook,
            Queen,
            KingSideCastle,
            QueenSideCastle,
            None,
        }

        public static void MovePiece(ref ulong pieces, int pieceType, Move move, ref ChessBoard board)
        {
            EnPassant(ref board, move.startPos, move.endPos, pieceType);
            FiftyMoveRuleCounter(ref board, pieceType, move.startPos, move.endPos);
            CastelingRights(ref board, pieceType, move.startPos, move.endPos);
            CheckForCapture(pieceType, move.endPos, ref board);
            bool canPromote = Promotion(pieceType, move, ref board);

            board.MoveCount++;

            board.WhiteToMove = !board.WhiteToMove;

            // Removes the bit from the position
            pieces &= ~(1UL << move.startPos);
            // Adds the bit to the new position
            if (!canPromote) pieces |= (1UL << move.endPos);

            DoCastleMoves(move.specialFlags, ref board, pieceType < 6);
            board.UpdateBitBoards();

            if (pieceType==5)
            {
                board.WhiteKingPos=move.endPos;
            }
            else if (pieceType==11)
            {
                board.BlackKingPos = move.endPos;
            }

            UpdateAttackBoard(ref board);

            ChessEngine.boardRenderer.UpdateBoard();
        }


        public static void SearchMovePiece(ref ulong pieces, int pieceType, Move move, ref ChessBoard board)
        {
            board.MoveCount++;
            board.WhiteToMove = !board.WhiteToMove;


            CastelingRights(ref board, pieceType, move.startPos, move.endPos);
            EnPassant(ref board, move.startPos, move.endPos, pieceType);
            CheckForCapture(pieceType, move.endPos, ref board);
            bool canPromote = Promotion(pieceType, move, ref board);

            pieces &= ~(1UL << move.startPos);
            if (!canPromote) pieces |= (1UL << move.endPos);

            DoCastleMoves(move.specialFlags, ref board, pieceType < 6);

            board.UpdateBitBoards();

            if (pieceType == 5)
            {
                board.WhiteKingPos = move.endPos;
            }
            else if (pieceType == 11)
            {
                board.BlackKingPos = move.endPos;
            }

            UpdateAttackBoard(ref board);
        }

        public static bool CheckForCapture(int pieceType, int pos, ref ChessBoard board)
        {
            int pieceTypeCheck = HelperFunctions.CheckIfPieceOnEveryBoard(pos, board);

            if (pieceTypeCheck != int.MaxValue)
            {

                HelperFunctions.SetBit(ref HelperFunctions.GetTypeBasedOnIndex(pieceTypeCheck, ref board), pos);
                return true;
            }
            return false;
        }

        // 0 = White Pawn, 1 = White Knigth, 2 = White Bishop, 3 = White Rook, 4 = White Queen, 5 = White King
        // 6 = Black Pawn, 7 = Black Knigth, 8 = Black Bishop, 9 = Black Rook, 10 = Black Queen, 11 = Black King

        public static Move[] GetLegalMoves(ref ChessBoard board, int pieceType, int position, bool testingMoves=false)
        {
            //moveGenTime.Start();
            List<Move> moves = new();
            // UInt64 blackAttackBoard = 0;

            switch (pieceType)
            {
                case 0:
                    if (HelperFunctions.GetBit(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        //moveGenTime.Stop();
                        return moves.ToArray();
                    }


                    if (position < 56 && HelperFunctions.GetBit(position + 8, board.AllPieces) == 0)
                    {
                        // Quiet move forward
                        if (position + 8 > 55) // Promotion
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 8)), true))
                            {
                                moves.Add(new((byte)position, (byte)(position + 8), SpecialFlags.Queen));
                                moves.Add(new((byte)position, (byte)(position + 8), SpecialFlags.Rook));
                                moves.Add(new((byte)position, (byte)(position + 8), SpecialFlags.Bishop));
                                moves.Add(new((byte)position, (byte)(position + 8), SpecialFlags.Knight));
                            }
                        }
                        else
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 8)), true))
                                moves.Add(new((byte)position, (byte)(position + 8)));
                        }

                        // Double move from starting rank
                        if (position >= 8 && position < 16 && HelperFunctions.GetBit(position + 16, board.AllPieces) == 0)
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 16)), true))
                                moves.Add(new((byte)position, (byte)(position + 16)));
                        }
                    }

                    // Captures
                    if (position < 56)
                    {
                        // Capture right
                        if (position % 8 != 7 && HelperFunctions.GetBit(position + 9, board.BlackPieces) == 1)
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 9)), true))
                            {
                                if (position + 9 > 55) // Promotion capture
                                {
                                    moves.Add(new((byte)position, (byte)(position + 9), SpecialFlags.Queen));
                                    moves.Add(new((byte)position, (byte)(position + 9), SpecialFlags.Rook));
                                    moves.Add(new((byte)position, (byte)(position + 9), SpecialFlags.Bishop));
                                    moves.Add(new((byte)position, (byte)(position + 9), SpecialFlags.Knight));
                                }
                                else
                                {
                                    moves.Add(new((byte)position, (byte)(position + 9)));
                                }
                            }
                        }

                        // Capture left
                        if (position % 8 != 0 && HelperFunctions.GetBit(position + 7, board.BlackPieces) == 1)
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 7)), true))
                            {
                                if (position + 7 > 55) // Promotion capture
                                {
                                    moves.Add(new((byte)position, (byte)(position + 7), SpecialFlags.Queen));
                                    moves.Add(new((byte)position, (byte)(position + 7), SpecialFlags.Rook));
                                    moves.Add(new((byte)position, (byte)(position + 7), SpecialFlags.Bishop));
                                    moves.Add(new((byte)position, (byte)(position + 7), SpecialFlags.Knight));
                                }
                                else
                                {
                                    moves.Add(new((byte)position, (byte)(position + 7)));
                                }
                            }
                        }
                    }

                    // En passant
                    if (position % 8 != 7 && position + 9 == board.EnPassantTargetSquare)
                    {
                        if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 9)), true))
                            moves.Add(new((byte)position, (byte)(position + 9)));
                    }
                    if (position % 8 != 0 && position + 7 == board.EnPassantTargetSquare)
                    {
                        if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 7)), true))
                            moves.Add(new((byte)position, (byte)(position + 7)));
                    }

                    break;
                case 1:
                    
                    if (HelperFunctions.GetBit(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        //moveGenTime.Stop();
                        return moves.ToArray();
                    }

                    if (position % 8 != 7 && position < 48)
                    {
                        if (HelperFunctions.GetBit(position + 17, board.WhitePieces) != 1)
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 17)), true))
                                moves.Add(new((byte)position, (byte)(position + 17)));
                        }
                    }
                    if (position % 8 != 0 && position < 48)
                    {
                        if (HelperFunctions.GetBit(position + 15, board.WhitePieces) != 1)
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 15)), true))
                                moves.Add(new((byte)position, (byte)(position + 15)));
                        }
                    }

                    if (position > 15)
                    {
                        if (position % 8 != 0)
                        {
                            if (HelperFunctions.GetBit(position - 17, board.WhitePieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new((byte)position, (byte)(position - 17)), true))
                                    moves.Add(new((byte)position, (byte)(position - 17)));
                            }
                        }
                        if (position % 8 != 7)
                        {
                            if (HelperFunctions.GetBit(position - 15, board.WhitePieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new((byte)position, (byte)(position - 15)), true))
                                    moves.Add(new((byte)position, (byte)(position - 15)));
                            }
                        }
                    }

                    if (position < 56)
                    {
                        if (position % 8 != 7 && position % 8 != 6)
                        {
                            if (HelperFunctions.GetBit(position + 10, board.WhitePieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 10)), true))
                                    moves.Add(new((byte)position, (byte)(position + 10)));
                            }
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            if (HelperFunctions.GetBit(position + 6, board.WhitePieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 6)), true))
                                    moves.Add(new((byte)position, (byte)(position + 6)));
                            }
                        }
                    }

                    if (position > 7)
                    {
                        if (position % 8 != 7 && position % 8 != 6)
                        {
                            if (HelperFunctions.GetBit(position - 6, board.WhitePieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 6)), true))
                                    moves.Add(new((byte)position, (byte)(position - 6)));
                            }
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            if (HelperFunctions.GetBit(position - 10, board.WhitePieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 10)), true))
                                    moves.Add(new((byte)position, (byte)(position - 10)));
                            }
                        }
                    }
                    break;

                   
                case 2:
                    BishopMovement(pieceType, position, board.WhitePieces, board.BlackPieces, ref moves, board);
                    break;

                case 3:
                    RookMovement(pieceType, position, board.WhitePieces, board.BlackPieces, ref moves, board);

                    break;

                case 4:
                    RookMovement(pieceType, position, board.WhitePieces, board.BlackPieces, ref moves, board);
                    BishopMovement(pieceType, position, board.WhitePieces, board.BlackPieces, ref moves, board);
                    break;
                case 5:

                    KingMovement(pieceType, position, board.WhitePieces, ref moves, board.BlackAttackBoard, board);

                    break;
                case 6:

                    if (HelperFunctions.GetBit(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                        return moves.ToArray();

                    // Forward one square
                    if (position > 7 && HelperFunctions.GetBit(position - 8, board.AllPieces) == 0)
                    {
                        if (position - 8 < 8) // Promotion rank
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 8)), false))
                            {
                                moves.Add(new((byte)position, (byte)(position - 8), SpecialFlags.Queen));
                                moves.Add(new((byte)position, (byte)(position - 8), SpecialFlags.Rook));
                                moves.Add(new((byte)position, (byte)(position - 8), SpecialFlags.Bishop));
                                moves.Add(new((byte)position, (byte)(position - 8), SpecialFlags.Knight));
                            }
                        }
                        else
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 8)), false))
                                moves.Add(new((byte)position, (byte)(position - 8)));
                        }

                        // Double forward move from starting rank
                        if (position >= 48 && position < 56 && HelperFunctions.GetBit(position - 16, board.AllPieces) == 0)
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 16)), false))
                                moves.Add(new((byte)position, (byte)(position - 16)));
                        }
                    }

                    // Diagonal captures
                    if (position > 7)
                    {
                        // Capture right
                        if (position % 8 != 7 && HelperFunctions.GetBit(position - 7, board.WhitePieces) == 1)
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 7)), false))
                            {
                                if (position - 7 < 8) // Promotion capture
                                {
                                    moves.Add(new((byte)position, (byte)(position - 7), SpecialFlags.Queen));
                                    moves.Add(new((byte)position, (byte)(position - 7), SpecialFlags.Rook));
                                    moves.Add(new((byte)position, (byte)(position - 7), SpecialFlags.Bishop));
                                    moves.Add(new((byte)position, (byte)(position - 7), SpecialFlags.Knight));
                                }
                                else
                                {
                                    moves.Add(new((byte)position, (byte)(position - 7)));
                                }
                            }
                        }

                        // Capture left
                        if (position % 8 != 0 && HelperFunctions.GetBit(position - 9, board.WhitePieces) == 1)
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 9)), false))
                            {
                                if (position - 9 < 8) // Promotion capture
                                {
                                    moves.Add(new((byte)position, (byte)(position - 9), SpecialFlags.Queen));
                                    moves.Add(new((byte)position, (byte)(position - 9), SpecialFlags.Rook));
                                    moves.Add(new((byte)position, (byte)(position - 9), SpecialFlags.Bishop));
                                    moves.Add(new((byte)position, (byte)(position - 9), SpecialFlags.Knight));
                                }
                                else
                                {
                                    moves.Add(new((byte)position, (byte)(position - 9)));
                                }
                            }
                        }
                    }

                    // En passant
                    if (position % 8 != 0 && position - 9 == board.EnPassantTargetSquare)
                    {
                        if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 9)), false))
                            moves.Add(new((byte)position, (byte)(position - 9)));
                    }
                    if (position % 8 != 7 && position - 7 == board.EnPassantTargetSquare)
                    {
                        if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 7)), false))
                            moves.Add(new((byte)position, (byte)(position - 7)));
                    }

                    break;
                case 7:
                    if (HelperFunctions.GetBit(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        //moveGenTime.Stop();
                        return moves.ToArray();
                    }

                    if (position % 8 != 7 && position < 48)
                    {
                        if (HelperFunctions.GetBit(position + 17, board.BlackPieces) != 1)
                        {
                            if (blocksCheck(board, pieceType, new((byte)position,(byte)(position+17)),false)) moves.Add(new((byte)position, (byte)(position + 17)));
                        }
                    }
                    if (position % 8 != 0 && position < 48)
                    {
                        if (HelperFunctions.GetBit(position + 15, board.BlackPieces) != 1)
                        {
                            if (blocksCheck(board, pieceType, new((byte)position, (byte)(position + 15)), false)) moves.Add(new((byte)position, (byte)(position + 15)));
                        }
                    }

                    if (position > 15)
                    {
                        if (position % 8 != 0)
                        {
                            if (HelperFunctions.GetBit(position - 17, board.BlackPieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new((byte)position, (byte)(position - 17)), false)) moves.Add(new((byte)position, (byte)(position - 17)));
                            }
                        }
                        if (position % 8 != 7)
                        {
                            if (HelperFunctions.GetBit(position - 15, board.BlackPieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new((byte)position, (byte)(position - 15)), false))
                                    moves.Add(new((byte)position, (byte)(position - 15)));
                            }
                        }
                    }

                    if (position < 56)
                    {
                        if (position % 8 != 7 && position % 8 != 6)
                        {
                            if (HelperFunctions.GetBit(position + 10, board.BlackPieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new((byte)position, (byte)(position + 10)), false))
                                    moves.Add(new((byte)position, (byte)(position + 10)));
                            }
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            if (HelperFunctions.GetBit(position + 6, board.BlackPieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new((byte)position, (byte)(position + 6)), false))
                                    moves.Add(new((byte)position, (byte)(position + 6)));
                            }
                        }
                    }

                    if (position > 7)
                    {
                        if (position % 8 != 7 && position % 8 != 6)
                        {
                            if (HelperFunctions.GetBit(position - 6, board.BlackPieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new((byte)position, (byte)(position - 6)), false))
                                    moves.Add(new((byte)position, (byte)(position - 6)));
                            }
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            if (HelperFunctions.GetBit(position - 10, board.BlackPieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new((byte)position, (byte)(position - 10)), false))
                                    moves.Add(new((byte)position, (byte)(position - 10)));
                            }
                        }
                    }
                    break;
                case 8:
                    BishopMovement(pieceType, position, board.BlackPieces, board.WhitePieces, ref moves, board);
                    break;
                case 9:
                    RookMovement(pieceType, position, board.BlackPieces, board.WhitePieces, ref moves, board);

                    break;
                case 10:
                    RookMovement(pieceType, position, board.BlackPieces, board.WhitePieces, ref moves, board);
                    BishopMovement(pieceType, position, board.BlackPieces, board.WhitePieces, ref moves, board);
                    break;
                case 11:

                    KingMovement(pieceType, position, board.BlackPieces, ref moves, board.WhiteAttackBoard, board);
                    break;

            }

            //moveGenTime.Stop();
            return moves.ToArray();
        }

        public static bool Promotion(int pieceType, Move move, ref ChessBoard board)
        {
            if (move.endPos < 56) { if (move.endPos < 8) { return false; } }


            if (pieceType==0&&move.endPos >55)
            {
                HelperFunctions.SetBit(ref board.WhitePawns, move.endPos);
                switch (move.specialFlags)
                {
                    case SpecialFlags.Knight:
                        HelperFunctions.SetBit(ref board.WhiteKnights, move.endPos, 1);
                        return true;
                    case SpecialFlags.Bishop:
                        HelperFunctions.SetBit(ref board.WhiteBishops, move.endPos, 1);
                        return true;
                    case SpecialFlags.Rook:
                        HelperFunctions.SetBit(ref board.WhiteRooks, move.endPos,1);
                        return true;
                    case SpecialFlags.Queen:
                        HelperFunctions.SetBit(ref board.WhiteQueens, move.endPos,1);
                        return true;
                }
            }

            if (pieceType == 6 && move.endPos < 8)
            {
                HelperFunctions.SetBit(ref board.BlackPawns, move.endPos);
                switch (move.specialFlags)
                {
                    case SpecialFlags.Knight:
                        HelperFunctions.SetBit(ref board.BlackKnights, move.endPos, 1);
                        return true;
                        
                    case SpecialFlags.Bishop:
                        HelperFunctions.SetBit(ref board.BlackBishops, move.endPos, 1);
                        return true;
                        
                    case SpecialFlags.Rook:
                        HelperFunctions.SetBit(ref board.BlackRooks, move.endPos, 1);
                        return true;
                        
                    case SpecialFlags.Queen:
                        HelperFunctions.SetBit(ref board.BlackQueens, move.endPos, 1);
                        return true;
                }
            }

            return false;
        }

        
        public static bool blocksCheck(ChessBoard board, int pieceType, Move move, bool white)
        {

            ChessBoard copy = (ChessBoard)board.Clone();

            SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref copy), pieceType, move, ref copy);
            //moveGenTime.Stop();

            UpdateAttackBoard(ref copy);
            if (white)
            {
                if (!copy.IsWhiteChecked())
                {
                    return true;
                }
            }
            else
            {
                if (!copy.IsBlackChecked())
                {
                    return true;
                }
            }

            return false;
        }

        public static void UpdateAttackBoardForAPiece(ref ChessBoard board, int pieceType, int position)
        {
            switch (pieceType)
            {
                case 0:
                    if (position%8!=0&&position<56)
                    {
                        board.WhiteAttackBoard |= 1UL << (position + 7);
                    }
                    if (position % 8 != 7&&position<56)
                    {
                        board.WhiteAttackBoard |= 1UL << (position + 9);
                    }
                    break;
                case 1:
                    AddKnightAttacks(position, ref board.WhiteAttackBoard);

                    break;
                case 2:
                    if (position < 56)
                    {
                        int updatedPosition = position + 9;  
                        while (updatedPosition < 64 && updatedPosition % 8 != 0)
                        {
                            board.WhiteAttackBoard |= 1UL << (int)updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetBit(updatedPosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition += 9;

                        }
                        updatedPosition = position + 7;
                        while (updatedPosition < 63 && updatedPosition % 8 != 7)
                        {
                            board.WhiteAttackBoard |= 1UL << updatedPosition ;
                            if (HelperFunctions.GetBit(updatedPosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetBit(updatedPosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition += 7;

                        }

                    }

                    if (position > 7)
                    {
                        int updatedPosition = position - 7;

                        while (updatedPosition > 0 && updatedPosition % 8 != 0)
                        {
                            board.WhiteAttackBoard |= 1UL << (int)updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            

                            if (HelperFunctions.GetBit(updatedPosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 7;
                        }
                        updatedPosition = position - 9;
                        while (updatedPosition >= 0 && updatedPosition % 8 != 7)
                        {
                            board.WhiteAttackBoard |= 1UL << (int)updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetBit(updatedPosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 9;
                        }
                    }

                    break;
                case 3:
                    if (position < 56)
                    {
                        int updatedPosition = position + 8;
                        while (updatedPosition - 8 < 56)
                        {
                            board.WhiteAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition += 8;
                        }
                    }
                    if (position >= 8)
                    {
                        int updatedPosition = position - 8;
                        while (updatedPosition + 8 >= 8)
                        {
                            board.WhiteAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 8;
                        }
                    }

                    if (position % 8 != 7)
                    {
                        int updatedPosition = (byte)(position + 1);
                        while (updatedPosition % 8 != 0)
                        {
                            board.WhiteAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition++;
                        }
                    }

                    if (position % 8 != 0)
                    {
                        uint updatedPosition = (uint)(position - 1);
                        while (updatedPosition % 8 != 7)
                        {

                            board.WhiteAttackBoard |= 1UL << (int)updatedPosition;
                            if (HelperFunctions.GetBit((int)updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition--;
                        }
                    }
                    break;
                case 4:
                    if (position < 56)
                    {
                        int updatedPosition = position + 8;
                        while (updatedPosition - 8 < 56)
                        {
                            board.WhiteAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition += 8;
                        }
                    }
                    if (position >= 8)
                    {
                        int updatedPosition = position - 8;
                        while (updatedPosition + 8 >= 8)
                        {
                            board.WhiteAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 8;
                        }
                    }

                    if (position % 8 != 7)
                    {
                        int updatedPosition = (byte)(position + 1);
                        while (updatedPosition % 8 != 0)
                        {
                            board.WhiteAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition++;
                        }
                    }

                    if (position % 8 != 0)
                    {
                        uint updatedPosition = (uint)(position - 1);
                        while (updatedPosition % 8 != 7)
                        {

                            board.WhiteAttackBoard |= 1UL << (int)updatedPosition;
                            if (HelperFunctions.GetBit((int)updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition--;
                        }
                    }

                    if (position < 56)
                    {
                        int updatedPosition = position + 9;
                        while (updatedPosition < 64 && updatedPosition % 8 != 0)
                        {
                            board.WhiteAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition += 9;

                        }
                        updatedPosition = position + 7;
                        while (updatedPosition < 63 && updatedPosition % 8 != 7)
                        {
                            board.WhiteAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition += 7;

                        }

                    }

                    if (position > 7)
                    {
                        int updatedPosition = position - 7;

                        while (updatedPosition > 0 && updatedPosition % 8 != 0)
                        {
                            board.WhiteAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 7;
                        }
                        updatedPosition = position - 9;
                        while (updatedPosition >= 0 && updatedPosition % 8 != 7)
                        {
                            board.WhiteAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 9;
                        }
                    }
                    break;
                case 5:

                    int[] directions = { 8, 9, 1, -7, -8, -9, -1, 7 };
                    foreach (int dir in directions)
                    {
                        int target = position + dir;

                        // Skip if move is off the board due to wrapping
                        if (target < 0 || target > 63)
                            continue;

                        // Prevent wrap-around on left/right edges
                        if ((dir == -9 || dir == -1 || dir == 7) && position % 8 == 0)
                            continue;
                        if ((dir == -7 || dir == 1 || dir == 9) && position % 8 == 7)
                            continue;

                        board.WhiteAttackBoard |= 1UL << target;
                    }
                    break;
                case 6:

                    if (position % 8 != 7&&position>7)
                    {
                        board.BlackAttackBoard |= 1UL << (position - 7);
                    }
                    if (position % 8 != 0&&position>7)
                    {
                        board.BlackAttackBoard |= 1UL << (position - 9);
                    }
                    break;
                case 7:
                    AddKnightAttacks(position, ref board.BlackAttackBoard);

                    break;
                case 8:
                    if (position < 56)
                    {
                        int updatedPosition = position + 9;
                        while (updatedPosition < 64 && updatedPosition % 8 != 0)
                        {
                            board.BlackAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition += 9;

                        }
                        updatedPosition = position + 7;
                        while (updatedPosition < 63 && updatedPosition % 8 != 7)
                        {
                            board.BlackAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition += 7;

                        }

                    }

                    if (position > 7)
                    {
                        int updatedPosition = position - 7;

                        while (updatedPosition > 0 && updatedPosition % 8 != 0)
                        {
                            board.BlackAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 7;
                        }
                        updatedPosition = position - 9;
                        while (updatedPosition >= 0 && updatedPosition % 8 != 7)
                        {
                            board.BlackAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 9;
                        }
                    }
                    break;
                case 9:
                    if (position < 56)
                    {
                        int updatedPosition = position + 8;
                        while (updatedPosition - 8 < 56)
                        {
                            board.BlackAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition += 8;
                        }
                    }
                    if (position >= 8)
                    {
                        int updatedPosition = position - 8;
                        while (updatedPosition + 8 >= 8)
                        {
                            board.BlackAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 8;
                        }
                    }

                    if (position % 8 != 7)
                    {
                        int updatedPosition = (byte)(position + 1);
                        while (updatedPosition % 8 != 0)
                        {
                            board.BlackAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition++;
                        }
                    }

                    if (position % 8 != 0)
                    {
                        uint updatedPosition = (uint)(position - 1);
                        while (updatedPosition % 8 != 7)
                        {

                            board.BlackAttackBoard |= 1UL << (int)updatedPosition;
                            if (HelperFunctions.GetBit((int)updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition--;
                        }
                    }
                    break;
                case 10:
                    if (position < 56)
                    {
                        int updatedPosition = position + 8;
                        while (updatedPosition - 8 < 56)
                        {
                            board.BlackAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition += 8;
                        }
                    }
                    if (position >= 8)
                    {
                        int updatedPosition = position - 8;
                        while (updatedPosition + 8 >= 8)
                        {
                            board.BlackAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 8;
                        }
                    }

                    if (position % 8 != 7)
                    {
                        int updatedPosition = (byte)(position + 1);
                        while (updatedPosition % 8 != 0)
                        {
                            board.BlackAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition++;
                        }
                    }

                    if (position % 8 != 0)
                    {
                        uint updatedPosition = (uint)(position - 1);
                        while (updatedPosition % 8 != 7)
                        {

                            board.BlackAttackBoard |= 1UL << (int)updatedPosition;
                            if (HelperFunctions.GetBit((int)updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition--;
                        }
                    }

                    if (position < 56)
                    {
                        int updatedPosition = position + 9;
                        while (updatedPosition < 64 && updatedPosition % 8 != 0)
                        {
                            board.BlackAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition += 9;

                        }
                        updatedPosition = position + 7;
                        while (updatedPosition < 63 && updatedPosition % 8 != 7)
                        {
                            board.BlackAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition += 7;

                        }

                    }

                    if (position > 7)
                    {
                        int updatedPosition = position - 7;

                        while (updatedPosition > 0 && updatedPosition % 8 != 0)
                        {
                            board.BlackAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 7;
                        }
                        updatedPosition = position - 9;
                        while (updatedPosition >= 0 && updatedPosition % 8 != 7)
                        {
                            board.BlackAttackBoard |= 1UL << updatedPosition;
                            if (HelperFunctions.GetBit(updatedPosition, board.AllPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 9;
                        }
                    }
                    
                    break;
                case 11:

                    int[] directions1 = { 8, 9, 1, -7, -8, -9, -1, 7 };
                    foreach (int dir in directions1)
                    {
                        int target = position + dir;

                        // Skip if move is off the board due to wrapping
                        if (target < 0 || target > 63)
                            continue;

                        // Prevent wrap-around on left/right edges
                        if ((dir == -9 || dir == -1 || dir == 7) && position % 8 == 0)
                            continue;
                        if ((dir == -7 || dir == 1 || dir == 9) && position % 8 == 7)
                            continue;

                        board.BlackAttackBoard |= 1UL << target;
                    }
                    break;
                default:
                    Debug.LogError("Invalid piece type");
                    break;
            }
        }

        private static void AddKnightAttacks(int pos, ref ulong attackBoard)
        {
            int[] offsets = { 17, 15, 10, 6, -6, -10, -15, -17 };

            foreach (int offset in offsets)
            {
                int target = pos + offset;
                if (target >= 0 && target < 64 && IsValidKnightMove(pos, target))
                    attackBoard |= 1UL << target;
            }
        }

        private static bool IsValidKnightMove(int from, int to)
        {
            int fileFrom = from % 8;
            int fileTo = to % 8;
            int df = Math.Abs(fileFrom - fileTo);
            int dr = Math.Abs((from / 8) - (to / 8));
            return df <= 2 && dr <= 2; // prevents wrap
        }


        public static void UpdateAttackBoard(ref ChessBoard board)
        {
            board.WhiteAttackBoard = 0;
            board.BlackAttackBoard = 0;
            for (byte i = 0; i < 64; i++)
            {
                int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(i, board);
                if (pieceType == int.MaxValue) continue;
                UpdateAttackBoardForAPiece(ref board, pieceType, i);
            }
        }

        public static void RookMovement(int pieceType, int position, ulong friendlyPieces, ulong enemyPieces, ref List<Move> moves, ChessBoard board)
        {
            if (HelperFunctions.GetBit(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
            {
                return;
            }
            if (position < 56)
            {
                int updatePosition = position + 8;
                while (updatePosition - 8 < 56)
                {
                    if (HelperFunctions.GetBit(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)updatePosition), pieceType > 5 ? false : true))
                        moves.Add(new((byte)position, (byte)updatePosition));
                    if (HelperFunctions.GetBit(updatePosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatePosition += 8;
                }
            }
            if (position >= 8)
            {
                int updatePosition = position - 8;
                while (updatePosition + 8 >= 8)
                {
                    if (HelperFunctions.GetBit(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)updatePosition), pieceType > 5 ? false : true))
                        moves.Add(new((byte)position, (byte)updatePosition));
                    if (HelperFunctions.GetBit(updatePosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatePosition -= 8;
                }
            }

            if (position % 8 != 7)
            {
                int updatePosition = (byte)(position + 1);
                while (updatePosition % 8 != 0)
                {
                    if (HelperFunctions.GetBit(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)updatePosition), pieceType > 5 ? false : true))
                        moves.Add(new((byte)position, (byte)updatePosition));
                    if (HelperFunctions.GetBit(updatePosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatePosition++;
                }
            }

            if (position % 8 != 0)
            {
                uint updatePosition = (uint)(position - 1);
                while (updatePosition % 8 != 7)
                {

                    if (HelperFunctions.GetBit((int)updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)(updatePosition)), pieceType > 5 ? false : true))
                        moves.Add(new((byte)position, (byte)(updatePosition)));
                    if (HelperFunctions.GetBit((int)updatePosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatePosition--;
                }
            }
        }

        public static void BishopMovement(int pieceType, int position, ulong friendlyPieces, ulong enemyPieces, ref List<Move> moves, ChessBoard board)
        {
            if (HelperFunctions.GetBit(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
            {
                return;
            }
            if (position < 56)
            {
                int updatePosition = position + 9;
                while (updatePosition < 64 && updatePosition % 8 != 0)
                {
                    if (HelperFunctions.GetBit(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)(updatePosition)), pieceType > 5 ? false : true)) moves.Add(new((byte)position, (byte)(updatePosition)));
                    if (HelperFunctions.GetBit(updatePosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatePosition += 9;

                }
                updatePosition = position + 7;
                while (updatePosition < 63 && updatePosition % 8 != 7)
                {
                    if (HelperFunctions.GetBit(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)(updatePosition)), pieceType > 5 ? false : true))
                        moves.Add(new((byte)position, (byte)(updatePosition)));
                    if (HelperFunctions.GetBit(updatePosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatePosition += 7;

                }

            }
            if (position > 7)
            {
                int updatedPosition = position - 7;

                while (updatedPosition > 0 && updatedPosition % 8 != 0)
                {
                    if (HelperFunctions.GetBit(updatedPosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)updatedPosition), pieceType > 5 ? false : true))
                        moves.Add(new((byte)position, (byte)updatedPosition));

                    if (HelperFunctions.GetBit(updatedPosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatedPosition -= 7;
                }
                updatedPosition = position - 9;
                while (updatedPosition >= 0 && updatedPosition % 8 != 7)
                {
                    if (HelperFunctions.GetBit(updatedPosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)updatedPosition), pieceType > 5 ? false : true))
                        moves.Add(new((byte)position, (byte)updatedPosition));
                    if (HelperFunctions.GetBit(updatedPosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatedPosition -= 9;
                }
            }
        }

        public static void KingMovement(int pieceType, int position, ulong friendlyPieces, ref List<Move> moves, ulong hostileAttackBoard, ChessBoard board)
        {
            if (HelperFunctions.GetBit(position, friendlyPieces) == 0)
                return;

            GetCastleMoves(pieceType < 6, ref moves, board);

            int[] directions = { 8, 9, 1, -7, -8, -9, -1, 7 };
            foreach (int dir in directions)
            {
                int target = position + dir;

                // Skip if move is off the board due to wrapping
                if (target < 0 || target > 63)
                    continue;

                // Prevent wrap-around on left/right edges
                if ((dir == -9 || dir == -1 || dir == 7) && position % 8 == 0)
                    continue;
                if ((dir == -7 || dir == 1 || dir == 9) && position % 8 == 7)
                    continue;


                if (HelperFunctions.GetBit(target, friendlyPieces) == 0 && HelperFunctions.GetBit(target, hostileAttackBoard)==0)
                {
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)(target)), pieceType > 5 ? false : true)) moves.Add(new Move((byte)position, (byte)target));
                }
            }
        }

        public static void GetCastleMoves(bool isWhite,ref List<Move> moves, ChessBoard board)
        {
            if (isWhite) {

                if (board.WhiteCanCastleKingside)
                {
                    if (HelperFunctions.GetBit(5, board.AllPieces) == 1 || HelperFunctions.GetBit(6, board.AllPieces) == 1) { }
                    else if (HelperFunctions.GetBit(5, board.BlackAttackBoard) == 1 || HelperFunctions.GetBit(6, board.BlackAttackBoard) == 1 || HelperFunctions.GetBit(4, board.BlackAttackBoard)==1) { }
                    else moves.Add(new Move(4, 6, SpecialFlags.KingSideCastle));
                }
                if (board.WhiteCanCastleQueenside)
                {
                    if (HelperFunctions.GetBit(1, board.AllPieces) == 1 || HelperFunctions.GetBit(2, board.AllPieces) == 1 || HelperFunctions.GetBit(3, board.AllPieces) == 1) { }
                    else if (HelperFunctions.GetBit(4, board.BlackAttackBoard) == 1 || HelperFunctions.GetBit(3, board.BlackAttackBoard) == 1 || HelperFunctions.GetBit(2, board.BlackAttackBoard) == 1) { }

                    else moves.Add(new Move(4, 2, SpecialFlags.QueenSideCastle));
                }
                return;
            }
            else
            {
                if (board.BlackCanCastleKingside)
                {
                    if (HelperFunctions.GetBit(61, board.AllPieces) == 1 || HelperFunctions.GetBit(62, board.AllPieces) == 1) { }
                    else if (HelperFunctions.GetBit(62, board.WhiteAttackBoard) == 1 || HelperFunctions.GetBit(61, board.WhiteAttackBoard) == 1 || HelperFunctions.GetBit(60, board.WhiteAttackBoard) == 1) { }
                    else moves.Add(new Move(60, 62, SpecialFlags.KingSideCastle));
                }
                if (board.BlackCanCastleQueenside)
                {
                    if (HelperFunctions.GetBit(57, board.AllPieces) == 1 || HelperFunctions.GetBit(58, board.AllPieces) == 1 || HelperFunctions.GetBit(59, board.AllPieces) == 1) { }
                    else if (HelperFunctions.GetBit(58, board.WhiteAttackBoard) == 1 || HelperFunctions.GetBit(59, board.WhiteAttackBoard) == 1 || HelperFunctions.GetBit(60, board.WhiteAttackBoard) == 1) { }

                    else moves.Add(new Move(60, 58, SpecialFlags.QueenSideCastle));
                }
            }
        }

        public static void DoCastleMoves(SpecialFlags castleType, ref ChessBoard board, bool isWhite)
        {
            if (isWhite)
            {
                if (castleType == SpecialFlags.KingSideCastle)
                {
                    // Remove the rook from position 7     2^7=128UL
                    board.WhiteRooks &= ~128UL;

                    // Move the rook to position 5       2^5=32UL
                    board.WhiteRooks |= 32UL;

                    return;
                }

                if (castleType == SpecialFlags.QueenSideCastle)
                {
                    // Remove the rook from position 0     2^0=1UL
                    board.WhiteRooks &= ~1UL;

                    // Move the rook to position 2       2^2=4UL
                    board.WhiteRooks |= 8UL;

                    return;
                }
            }

            else
            {
                if (castleType == SpecialFlags.KingSideCastle)
                {
                    // Remove the rook from position 63     2^63=9223372036854775808UL
                    board.BlackRooks &= ~9223372036854775808UL;

                    // Move the rook to position 61       2^61=2305843009213693952UL
                    board.BlackRooks |= 2305843009213693952UL;

                    return;
                }
                if (castleType == SpecialFlags.QueenSideCastle)
                {
                    // Remove the rook from position 56     2^56=72057594037927936UL
                    board.BlackRooks &= ~72057594037927936UL;

                    // Move the rook to position 59       2^59=576460752303423488
                    board.BlackRooks |= 576460752303423488UL;

                    return;
                }
            }
        }

        public static void EnPassant(ref ChessBoard board, byte startPosition, byte endPosition, int pieceType)
        {
            // Value set to 255 so that it is not on board if en passant is not possible

            if (pieceType == 0)
            {

                if (endPosition == board.EnPassantTargetSquare)
                {
                    CheckForCapture(pieceType, endPosition - 8, ref board);
                    board.EnPassantTargetSquare = 255;
                }


                if (endPosition == startPosition + 16)
                {
                    board.EnPassantTargetSquare = (byte)(endPosition - (byte)8);
                }
                else
                {
                    board.EnPassantTargetSquare = 255;
                }
            }
            else if (pieceType == 6)
            {
                if (endPosition == board.EnPassantTargetSquare)
                {
                    CheckForCapture(pieceType, endPosition + 8, ref board);
                    board.EnPassantTargetSquare = 255;
                }



                if (endPosition == startPosition - 16)
                {
                    board.EnPassantTargetSquare = (byte)(endPosition + (byte)8);
                }
                else
                {
                    board.EnPassantTargetSquare = 255;
                }
            }
            else
            {
                board.EnPassantTargetSquare = 255;
            }
        }

        public static void CastelingRights(ref ChessBoard board, int pieceType, byte startPosition, byte endPosition)
        {
            if (!board.WhiteCanCastleQueenside && !board.WhiteCanCastleKingside && !board.BlackCanCastleKingside && !board.BlackCanCastleQueenside) return;
            // Remove all castling rights for white if king is moved
            if (pieceType == 5)
            {
                board.WhiteCanCastleKingside = false;
                board.WhiteCanCastleQueenside = false;
                return;
            }
            // Remove all castling rights for black if king is moved
            else if (pieceType == 11)
            {
                board.BlackCanCastleKingside = false;
                board.BlackCanCastleQueenside = false;
                return;
            }

            int CapturePieceType = HelperFunctions.CheckIfPieceOnEveryBoard(endPosition, board);
            if (CapturePieceType == 3)
            {
                if (endPosition == 0)
                {
                    board.WhiteCanCastleQueenside = false;
                }
                if (endPosition == 7)
                {
                    board.WhiteCanCastleKingside = false;
                }
            }

            else if (CapturePieceType == 9)
            {
                if (endPosition == 56)
                {
                    board.BlackCanCastleQueenside = false;
                }
                if (endPosition == 63)
                {
                    board.BlackCanCastleKingside = false;
                }
            }

            //Now white rooks
            if (pieceType == 3)
            {
                if (startPosition == 0)
                {
                    board.WhiteCanCastleQueenside = false;
                }
                if (startPosition == 7)
                {
                    board.WhiteCanCastleKingside = false;
                }
            }

            //Now black rooks
            else if (pieceType == 9)
            {
                if (startPosition == 56)
                {
                    board.BlackCanCastleQueenside = false;
                }
                if (startPosition == 63)
                {
                    board.BlackCanCastleKingside = false;
                }
            }
        }
        public static void MakeAIMove(bool IsWhite)
        {
            Move move = Search.IterativeSearchAllMoves(ChessEngine.depth, IsWhite, ChessEngine.board);

            ChessEngine.board.UpdateBitBoards();
            ChessEngine.boardRenderer.UpdateBoard();

            int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(move.startPos, ChessEngine.board);

            Debug.Log("AI moving from " + move.startPos + " to " + move.endPos + " with a piece type of " + pieceType);

            MovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType), pieceType, move, ref ChessEngine.board);
            ChessEngine.prevMove = move;
        }

        public static MovePieces.Move[] GetMovesForBlackOrWhite(bool IsWhite)
        {
            List<Move> moves = new();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int TEMPpieceType = HelperFunctions.CheckIfPieceOnEveryBoard(i * 8 + j);
                    if (TEMPpieceType == int.MaxValue)
                    {
                        continue;
                    }
                    if (IsWhite)
                    {
                        if (TEMPpieceType > 5)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (TEMPpieceType < 6)
                        {
                            continue;
                        }
                    }

                    Move[] legalMoves = GetLegalMoves(ref ChessEngine.board, (byte)TEMPpieceType, (byte)(i * 8 + j));

                    if (legalMoves.Length <= 0)
                    {
                        continue;
                    }


                    for (int z = 0; z < legalMoves.Length; z++)
                    {
                        moves.Add(moves[z]);
                    }
                }
            }
            return moves.ToArray();
        }

        public static void FiftyMoveRuleCounter(ref ChessBoard board, int pieceType, byte startPosition, byte endPosition)
        {
            bool hasCaptured = false;

            if (pieceType == 0)
            {
                if (endPosition == startPosition + 9 || endPosition == startPosition + 7)
                {
                    hasCaptured = CheckForCapture(pieceType, endPosition, ref board);
                }
            }

            else if (pieceType == 6)
            {
                if (endPosition == startPosition - 9 || endPosition == startPosition - 7)
                {
                    hasCaptured = CheckForCapture(pieceType, endPosition, ref board);
                }
            }

            else
            {
                hasCaptured = CheckForCapture(pieceType, endPosition, ref board);
            }

            if (pieceType != 0 && pieceType != 6)
            {
                if (!hasCaptured)
                {
                    board.FiftyMoveRule += (float)0.5;
                }
                else
                {
                    board.FiftyMoveRule = 0;
                }
            }
            else
            {
                board.FiftyMoveRule = 0;
            }

            if (board.FiftyMoveRule >= 50)
            {
                Debug.Log("Draw by 50-Move rule");
            }
            Debug.Log(board.FiftyMoveRule);
        }

        public static Move[] GetMovesForBlackOrWhite(bool IsWhite, ChessBoard board)
        {
            Span<Move> moves = stackalloc Move[255]; 
            int counter = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int TEMPpieceType = HelperFunctions.CheckIfPieceOnEveryBoard(i * 8 + j, board);
                    if (TEMPpieceType == int.MaxValue)
                    {
                        continue;
                    }
                    if (IsWhite)
                    {
                        if (TEMPpieceType > 5)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (TEMPpieceType < 6)
                        {
                            continue;
                        }
                    }

                    Move[] legalMoves = GetLegalMoves(ref board, (byte)TEMPpieceType, (byte)(i * 8 + j));

                    if (legalMoves.Length <= 0)
                    {
                        continue;
                    }


                    for (int z = 0; z < legalMoves.Length; z++)
                    {
                        moves[counter]=legalMoves[z];
                        counter++;
                    }
                }
            }
            
            return moves.ToArray().Where(move=>move.startPos!=move.endPos).ToArray();
        }

        public struct Move
        {
            public byte startPos;
            public byte endPos;

            public SpecialFlags specialFlags;

            public Move(byte startPos1, byte endPos1)
            {
                startPos = startPos1;
                endPos = endPos1;
                specialFlags = SpecialFlags.None;
            }

            public Move(byte _startPos, byte _endPos, SpecialFlags _specialFlags)
            {
                startPos = _startPos;
                endPos = _endPos;
                specialFlags = _specialFlags;
            }

            public override string ToString()
            {
                return $"From {startPos} to {endPos} with {specialFlags.ToString()}";
            }
        }
    }
}