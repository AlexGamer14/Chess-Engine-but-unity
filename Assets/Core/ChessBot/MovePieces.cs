using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ChessEngine
{
    public class MovePieces
    {
        public enum PromotionType
        {
            Knight,
            Bishop,
            Rook,
            Queen,
            None
        }

        public void MovePiece(ref ulong pieces, int pieceType, Move move, ref ChessBoard board)
        {
            EnPassant(ref board, move.startPos, move.endPos, pieceType);
            FiftyMoveRuleCounter(ref board, pieceType, move.startPos, move.endPos);
            CastelingRights(ref board, pieceType, move.startPos, move.endPos);
            CheckForCapture(pieceType, move.endPos, ref board);
            Promotion(pieceType, move, ref board);

            board.MoveCount++;

            board.WhiteToMove = !board.WhiteToMove;

            pieces = pieces & ~(ulong)Math.Pow(2, move.startPos);
            pieces = pieces | (ulong)Math.Pow(2, move.endPos);
            board.UpdateBitBoards();


            UpdateAttackBoard(ref board);

            ChessEngine.boardRenderer.UpdateBoard();
        }


        public void SearchMovePiece(ref ulong pieces, int pieceType, Move move, ref ChessBoard board)
        {
            if (pieceType == 0)
            {
                if (move.endPos == move.startPos + 16)
                {
                    board.EnPassantTargetSquare = (byte)(move.endPos - (byte)8);
                }
                if (move.endPos == board.EnPassantTargetSquare)
                {
                    CheckForCapture(pieceType, move.endPos - 8, ref board);
                    board.EnPassantTargetSquare = 255;
                }
                else
                {
                    board.EnPassantTargetSquare = 255;
                }
            }
            if (pieceType == 6)
            {
                if (move.endPos == move.startPos - 16)
                {
                    board.EnPassantTargetSquare = (byte)(move.startPos + (byte)8);
                }
                if (move.endPos == board.EnPassantTargetSquare)
                {
                    CheckForCapture(pieceType, move.endPos + 8, ref board);
                    board.EnPassantTargetSquare = 255;
                }
                else
                {
                    board.EnPassantTargetSquare = 255;
                }
            }
            else
            {
                board.EnPassantTargetSquare = byte.MaxValue;
            }


            CheckForCapture(pieceType, move.endPos, ref board);
            Promotion(pieceType, move, ref board);

            pieces = pieces & ~(ulong)Math.Pow(2, move.startPos);
            pieces = pieces | (ulong)Math.Pow(2, move.endPos);
            board.UpdateBitBoards();



            UpdateAttackBoard(ref board);

            
        }

        public bool CheckForCapture(int pieceType, int pos, ref ChessBoard board)
        {
            int pieceTypeCheck = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, pos, board);

            if (pieceTypeCheck != int.MaxValue)
            {

                HelperFunctions.SetBit(ref HelperFunctions.GetTypeBasedOnIndex(pieceTypeCheck, ref board), pos);
                return true;
            }
            return false;
        }

        // 0 = White Pawn, 1 = White Knigth, 2 = White Bishop, 3 = White Rook, 4 = White Queen, 5 = White King
        // 6 = Black Pawn, 7 = Black Knigth, 8 = Black Bishop, 9 = Black Rook, 10 = Black Queen, 11 = Black King

        public Move[] GetLegalMoves(ref ChessBoard board, int pieceType, int position, bool testingMoves=false)
        {
            List<Move> moves = new();
            // UInt64 blackAttackBoard = 0;

            switch (pieceType)
            {
                case 0:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        return moves.ToArray();
                    }

                    
                    if (position < 56 && HelperFunctions.GetByte(position + 8, board.AllPieces) == 0)
                    {
                        if (position + 8 > 55)
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 8)), true))
                            {
                                moves.Add(new Move((byte)position, (byte)(position + 8), PromotionType.Queen));
                                moves.Add(new Move((byte)position, (byte)(position + 8), PromotionType.Rook));
                                moves.Add(new Move((byte)position, (byte)(position + 8), PromotionType.Bishop));
                                moves.Add(new Move((byte)position, (byte)(position + 8), PromotionType.Knight));
                            }
                        }
                        else
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 8)), true)) moves.Add(new Move((byte)position, (byte)(position + 8)));
                        }
                        if (position > 7 && position < 16 && position < 48 && HelperFunctions.GetByte(position + 16, board.AllPieces) == 0)
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 16)), true)) moves.Add(new((byte)position, (byte)(position + 16)));
                        }
                    }
                    if (position < 56)
                    {
                        if (position % 8 != 7)
                        {
                            if (HelperFunctions.GetByte(position + 9, board.BlackPieces) == 1)
                            {
                                if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 9)), true))  moves.Add(new((byte)position, (byte)(position + 9)));

                            }
                        }
                        if (position % 8 != 0)
                        {

                            if (HelperFunctions.GetByte(position + 7, board.BlackPieces) == 1)
                            {
                                if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 7)), true))
                                    moves.Add(new((byte)position, (byte)(position + 7)));
                            }
                        }

                    }
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
                    
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        return moves.ToArray();
                    }

                    if (position % 8 != 7 && position < 48)
                    {
                        if (HelperFunctions.GetByte(position + 17, board.WhitePieces) != 1)
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 17)), true))
                                moves.Add(new((byte)position, (byte)(position + 17)));
                        }
                    }
                    if (position % 8 != 0 && position < 48)
                    {
                        if (HelperFunctions.GetByte(position + 15, board.WhitePieces) != 1)
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 15)), true))
                                moves.Add(new((byte)position, (byte)(position + 15)));
                        }
                    }

                    if (position > 15)
                    {
                        if (position % 8 != 0)
                        {
                            if (HelperFunctions.GetByte(position - 17, board.WhitePieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new((byte)position, (byte)(position - 17)), true))
                                    moves.Add(new((byte)position, (byte)(position - 17)));
                            }
                        }
                        if (position % 8 != 7)
                        {
                            if (HelperFunctions.GetByte(position - 15, board.WhitePieces) != 1)
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
                            if (HelperFunctions.GetByte(position + 10, board.WhitePieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position + 10)), true))
                                    moves.Add(new((byte)position, (byte)(position + 10)));
                            }
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            if (HelperFunctions.GetByte(position + 6, board.WhitePieces) != 1)
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
                            if (HelperFunctions.GetByte(position - 6, board.WhitePieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 6)), true))
                                    moves.Add(new((byte)position, (byte)(position - 6)));
                            }
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            if (HelperFunctions.GetByte(position - 10, board.WhitePieces) != 1)
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
                    
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        return moves.ToArray();
                    }

                    if (position > 7 && HelperFunctions.GetByte(position - 8, board.AllPieces) == 0)
                    {
                        if (position - 8 < 8)
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 8)), false))
                            {
                                moves.Add(new Move((byte)position, (byte)(position - 8), PromotionType.Queen));
                                moves.Add(new Move((byte)position, (byte)(position - 8), PromotionType.Rook));
                                moves.Add(new Move((byte)position, (byte)(position - 8), PromotionType.Bishop));
                                moves.Add(new Move((byte)position, (byte)(position - 8), PromotionType.Knight));
                            }
                        }
                        else
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 8)), false)) moves.Add(new Move((byte)position, (byte)(position - 8)));
                        }

                        // Forward move by 2 from starting rank
                        if (position >= 48 && position < 56 && HelperFunctions.GetByte(position - 16, board.AllPieces) == 0)
                        {
                            if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 16)), false))  moves.Add(new((byte)position, (byte)(position - 16)));
                        }
                    }

                    // Diagonal captures and attack board
                    if (position > 7)
                    {
                        if (position % 8 != 7)
                        {
                            if (HelperFunctions.GetByte(position - 7, board.WhitePieces) == 1)
                            {
                                if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 7)), false))  moves.Add(new((byte)position, (byte)(position - 7)));
                            }
                        }

                        if (position % 8 != 0)
                        {
                            if (HelperFunctions.GetByte(position - 9, board.WhitePieces) == 1)
                            {
                                if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 9)), false)) moves.Add(new((byte)position, (byte)(position - 9)));
                            }
                        }
                    }

                    // En passant
                    if (position % 8 != 0 && position - 9 == board.EnPassantTargetSquare)
                    {
                        if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 9)), false)) moves.Add(new((byte)position, (byte)(position - 9)));
                    }
                    if (position % 8 != 7 && position - 7 == board.EnPassantTargetSquare)
                    {
                        if (blocksCheck(board, pieceType, new Move((byte)position, (byte)(position - 7 )), false)) moves.Add(new((byte)position, (byte)(position - 7)));
                    }
                    break;

                case 7:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        return moves.ToArray();
                    }

                    if (position % 8 != 7 && position < 48)
                    {
                        if (HelperFunctions.GetByte(position + 17, board.BlackPieces) != 1)
                        {
                            if (blocksCheck(board, pieceType, new((byte)position,(byte)(position+17)),false)) moves.Add(new((byte)position, (byte)(position + 17)));
                        }
                    }
                    if (position % 8 != 0 && position < 48)
                    {
                        if (HelperFunctions.GetByte(position + 15, board.BlackPieces) != 1)
                        {
                            if (blocksCheck(board, pieceType, new((byte)position, (byte)(position + 15)), false)) moves.Add(new((byte)position, (byte)(position + 15)));
                        }
                    }

                    if (position > 15)
                    {
                        if (position % 8 != 0)
                        {
                            if (HelperFunctions.GetByte(position - 17, board.BlackPieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new((byte)position, (byte)(position - 17)), false)) moves.Add(new((byte)position, (byte)(position - 17)));
                            }
                        }
                        if (position % 8 != 7)
                        {
                            if (HelperFunctions.GetByte(position - 15, board.BlackPieces) != 1)
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
                            if (HelperFunctions.GetByte(position + 10, board.BlackPieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new((byte)position, (byte)(position + 10)), false))
                                    moves.Add(new((byte)position, (byte)(position + 10)));
                            }
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            if (HelperFunctions.GetByte(position + 6, board.BlackPieces) != 1)
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
                            if (HelperFunctions.GetByte(position - 6, board.BlackPieces) != 1)
                            {
                                if (blocksCheck(board, pieceType, new((byte)position, (byte)(position - 6)), false))
                                    moves.Add(new((byte)position, (byte)(position - 6)));
                            }
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            if (HelperFunctions.GetByte(position - 10, board.BlackPieces) != 1)
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

            return moves.ToArray();
        }

        public void Promotion(int pieceType, Move move, ref ChessBoard board)
        {
            if (move.endPos < 56) { if (move.endPos < 8) return; }


            if (pieceType==0&&move.endPos >55)
            {
                HelperFunctions.SetBit(ref board.WhitePawns, move.endPos);
                switch (move.promotionType)
                {
                    case PromotionType.Knight:
                        HelperFunctions.SetBit(ref board.WhiteKnights, move.endPos, 1);
                        break;
                    case PromotionType.Bishop:
                        HelperFunctions.SetBit(ref board.WhiteBishops, move.endPos, 1);
                        break;
                    case PromotionType.Rook:
                        HelperFunctions.SetBit(ref board.WhiteRooks, move.endPos,1);
                        break;
                    case PromotionType.Queen:
                        HelperFunctions.SetBit(ref board.WhiteQueens, move.endPos,1);
                        break;
                }
            }

            if (pieceType == 6 && move.endPos < 8)
            {
                HelperFunctions.SetBit(ref board.BlackPawns, move.endPos);
                switch (move.promotionType)
                {
                    case PromotionType.Knight:
                        HelperFunctions.SetBit(ref board.BlackKnights, move.endPos, 1);
                        break;
                    case PromotionType.Bishop:
                        HelperFunctions.SetBit(ref board.BlackBishops, move.endPos, 1);
                        break;
                    case PromotionType.Rook:
                        HelperFunctions.SetBit(ref board.BlackRooks, move.endPos, 1);
                        break;
                    case PromotionType.Queen:
                        HelperFunctions.SetBit(ref board.BlackQueens, move.endPos, 1);
                        break;
                }
            }
        }


        public bool blocksCheck(ChessBoard board, int pieceType, Move move, bool white)
        {
            ChessBoard copy = (ChessBoard)board.Clone();

            SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref copy), pieceType, move, ref copy);

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
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        return;
                    }

                    if (position%8!=0&&position<56)
                    {
                        board.WhiteAttackBoard[position+7] = true;
                    }
                    if (position % 8 != 7&&position<56)
                    {
                        board.WhiteAttackBoard[position + 9] = true;
                    }
                    break;
                case 1:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        return;
                    }
                    if (position % 8 != 7 && position < 48)
                    {
                        board.WhiteAttackBoard[position + 17] = true;
                    }
                    if (position % 8 != 0 && position < 48)
                    {
                        board.WhiteAttackBoard[position + 15] = true;
                    }

                    if (position > 15)
                    {
                        if (position % 8 != 0)
                        {
                            board.WhiteAttackBoard[position - 17] = true;
                        }
                        if (position % 8 != 7)
                        {
                            board.WhiteAttackBoard[position - 15] = true;
                        }
                    }

                    if (position < 56)
                    {
                        if (position % 8 != 7 && position % 8 != 6)
                        {
                            board.WhiteAttackBoard[position + 10] = true;
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            board.WhiteAttackBoard[position + 6] = true;
                        }
                    }

                    if (position > 7)
                    {
                        if (position % 8 != 7 && position % 8 != 6)
                        {
                            board.WhiteAttackBoard[position - 6] = true;
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            board.WhiteAttackBoard[position - 10] = true;
                        }
                    }

                    break;
                case 2:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        return;
                    }
                    if (position < 56)
                    {
                        int updatePosition = position + 9;  
                        while (updatePosition < 64 && updatePosition % 8 != 0)
                        {
                            board.WhiteAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatePosition += 9;

                        }
                        updatePosition = position + 7;
                        while (updatePosition < 63 && updatePosition % 8 != 7)
                        {
                            board.WhiteAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
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
                            board.WhiteAttackBoard[updatedPosition] = true;
                            if (HelperFunctions.GetByte(updatedPosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            

                            if (HelperFunctions.GetByte(updatedPosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 7;
                        }
                        updatedPosition = position - 9;
                        while (updatedPosition >= 0 && updatedPosition % 8 != 7)
                        {
                            board.WhiteAttackBoard[updatedPosition] = true;
                            if (HelperFunctions.GetByte(updatedPosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte(updatedPosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 9;
                        }
                    }

                    break;
                case 3:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        return;
                    }
                    if (position < 56)
                    {
                        int updatePosition = position + 8;
                        while (updatePosition - 8 < 56)
                        {
                            board.WhiteAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
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
                            board.WhiteAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
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
                            board.WhiteAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
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

                            board.WhiteAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte((int)updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte((int)updatePosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatePosition--;
                        }
                    }
                    break;
                case 4:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        return;
                    }
                    if (position < 56)
                    {
                        int updatePosition = position + 8;
                        while (updatePosition - 8 < 56)
                        {
                            board.WhiteAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }

                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
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
                            board.WhiteAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }

                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
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
                            board.WhiteAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
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

                            board.WhiteAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte((int)updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte((int)updatePosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatePosition--;
                        }
                    }

                    if (position < 56)
                    {
                        int updatePosition = position + 9;
                        while (updatePosition < 64 && updatePosition % 8 != 0)
                        {
                            board.WhiteAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatePosition += 9;

                        }
                        updatePosition = position + 7;
                        while (updatePosition < 63 && updatePosition % 8 != 7)
                        {
                            board.WhiteAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
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
                            board.WhiteAttackBoard[updatedPosition] = true;
                            if (HelperFunctions.GetByte(updatedPosition, board.WhitePieces) == 1)
                            {
                                break;
                            }


                            if (HelperFunctions.GetByte(updatedPosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 7;
                        }
                        updatedPosition = position - 9;
                        while (updatedPosition >= 0 && updatedPosition % 8 != 7)
                        {
                            board.WhiteAttackBoard[updatedPosition] = true;
                            if (HelperFunctions.GetByte(updatedPosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte(updatedPosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 9;
                        }
                    }
                    break;
                case 5:
                    if (HelperFunctions.GetByte(position, board.WhitePieces) == 0)
                        return;

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

                        board.WhiteAttackBoard[target] = true;
                    }
                    break;
                case 6:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        return;
                    }

                    if (position % 8 != 7&&position>7)
                    {
                        board.BlackAttackBoard[position - 7] = true;
                    }
                    if (position % 8 != 0&&position>7)
                    {
                        board.BlackAttackBoard[position - 9] = true;
                    }
                    break;
                case 7:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        return;
                    }
                    if (position % 8 != 7 && position < 48)
                    {
                        board.BlackAttackBoard[position + 17] = true;
                    }
                    if (position % 8 != 0 && position < 48)
                    {
                        board.BlackAttackBoard[position + 15] = true;
                    }

                    if (position > 15)
                    {
                        if (position % 8 != 0)
                        {
                            board.BlackAttackBoard[position - 17] = true;
                        }
                        if (position % 8 != 7)
                        {
                            board.BlackAttackBoard[position - 15] = true;
                        }
                    }

                    if (position < 56)
                    {
                        if (position % 8 != 7 && position % 8 != 6)
                        {
                            board.BlackAttackBoard[position + 10] = true;
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            board.BlackAttackBoard[position + 6] = true;
                        }
                    }

                    if (position > 7)
                    {
                        if (position % 8 != 7 && position % 8 != 6)
                        {
                            board.BlackAttackBoard[position - 6] = true;
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            board.BlackAttackBoard[position - 10] = true;
                        }
                    }

                    break;
                case 8:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        return;
                    }
                    if (position < 56)
                    {
                        int updatePosition = position + 9;
                        while (updatePosition < 64 && updatePosition % 8 != 0)
                        {
                            board.BlackAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatePosition += 9;

                        }
                        updatePosition = position + 7;
                        while (updatePosition < 63 && updatePosition % 8 != 7)
                        {
                            board.BlackAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
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
                            board.BlackAttackBoard[updatedPosition] = true;
                            if (HelperFunctions.GetByte(updatedPosition, board.WhitePieces) == 1)
                            {
                                break;
                            }


                            if (HelperFunctions.GetByte(updatedPosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 7;
                        }
                        updatedPosition = position - 9;
                        while (updatedPosition >= 0 && updatedPosition % 8 != 7)
                        {
                            board.BlackAttackBoard[updatedPosition] = true;
                            if (HelperFunctions.GetByte(updatedPosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte(updatedPosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 9;
                        }
                    }
                    break;
                case 9:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        return;
                    }
                    if (position < 56)
                    {
                        int updatePosition = position + 8;
                        while (updatePosition - 8 < 56)
                        {
                            board.BlackAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
                            {
                                break;
                            }

                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
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
                            board.BlackAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
                            {
                                break;
                            }

                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
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
                            board.BlackAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
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

                            board.BlackAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte((int)updatePosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte((int)updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            updatePosition--;
                        }
                    }
                    break;
                case 10:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        return;
                    }
                    if (position < 56)
                    {
                        int updatePosition = position + 8;
                        while (updatePosition - 8 < 56)
                        {
                            board.BlackAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
                            {
                                break;
                            }

                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
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
                            board.BlackAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
                            {
                                break;
                            }

                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
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
                            board.BlackAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
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

                            board.BlackAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte((int)updatePosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte((int)updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            updatePosition--;
                        }
                    }

                    if (position < 56)
                    {
                        int updatePosition = position + 9;
                        while (updatePosition < 64 && updatePosition % 8 != 0)
                        {
                            board.BlackAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            updatePosition += 9;

                        }
                        updatePosition = position + 7;
                        while (updatePosition < 63 && updatePosition % 8 != 7)
                        {
                            board.BlackAttackBoard[updatePosition] = true;
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
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
                            board.BlackAttackBoard[updatedPosition] = true;
                            if (HelperFunctions.GetByte(updatedPosition, board.BlackPieces) == 1)
                            {
                                break;
                            }


                            if (HelperFunctions.GetByte(updatedPosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 7;
                        }
                        updatedPosition = position - 9;
                        while (updatedPosition >= 0 && updatedPosition % 8 != 7)
                        {
                            board.BlackAttackBoard[updatedPosition] = true;
                            if (HelperFunctions.GetByte(updatedPosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            if (HelperFunctions.GetByte(updatedPosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 9;
                        }
                    }
                    
                    break;
                case 11:
                    if (HelperFunctions.GetByte(position, board.BlackPieces) == 0)
                        return;

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

                        board.BlackAttackBoard[target] = true;
                    }
                    break;
                default:
                    Debug.LogError("Invalid piece type");
                    break;
            }
        }

        public static void UpdateAttackBoard(ref ChessBoard board)
        {
            board.WhiteAttackBoard = new bool[64];
            board.BlackAttackBoard = new bool[64];
            for (byte i = 0; i < 64; i++)
            {
                int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, i, board);
                if (pieceType == int.MaxValue) continue;
                UpdateAttackBoardForAPiece(ref board, pieceType, i);
            }
        }

        public void RookMovement(int pieceType, int position, ulong friendlyPieces, ulong enemyPieces, ref List<Move> moves, ChessBoard board)
        {
            if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
            {
                return;
            }
            if (position < 56)
            {
                int updatePosition = position + 8;
                while (updatePosition - 8 < 56)
                {
                    if (HelperFunctions.GetByte(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)updatePosition), pieceType > 5 ? false : true))
                        moves.Add(new((byte)position, (byte)updatePosition));
                    if (HelperFunctions.GetByte(updatePosition, enemyPieces) == 1)
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
                    if (HelperFunctions.GetByte(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)updatePosition), pieceType > 5 ? false : true))
                        moves.Add(new((byte)position, (byte)updatePosition));
                    if (HelperFunctions.GetByte(updatePosition, enemyPieces) == 1)
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
                    if (HelperFunctions.GetByte(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)updatePosition), pieceType > 5 ? false : true))
                        moves.Add(new((byte)position, (byte)updatePosition));
                    if (HelperFunctions.GetByte(updatePosition, enemyPieces) == 1)
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

                    if (HelperFunctions.GetByte((int)updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)(updatePosition)), pieceType > 5 ? false : true))
                        moves.Add(new((byte)position, (byte)(updatePosition)));
                    if (HelperFunctions.GetByte((int)updatePosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatePosition--;
                }
            }
        }

        public void BishopMovement(int pieceType, int position, ulong friendlyPieces, ulong enemyPieces, ref List<Move> moves, ChessBoard board)
        {
            if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
            {
                return;
            }
            if (position < 56)
            {
                int updatePosition = position + 9;
                while (updatePosition < 64 && updatePosition % 8 != 0)
                {;
                    if (HelperFunctions.GetByte(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)(updatePosition)), pieceType > 5 ? false : true)) moves.Add(new((byte)position, (byte)(updatePosition)));
                    if (HelperFunctions.GetByte(updatePosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatePosition += 9;

                }
                updatePosition = position + 7;
                while (updatePosition < 63 && updatePosition % 8 != 7)
                {
                    if (HelperFunctions.GetByte(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)(updatePosition)), pieceType > 5 ? false : true))
                        moves.Add(new((byte)position, (byte)(updatePosition)));
                    if (HelperFunctions.GetByte(updatePosition, enemyPieces) == 1)
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
                    if (HelperFunctions.GetByte(updatedPosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)updatedPosition), pieceType > 5 ? false : true))
                        moves.Add(new((byte)position, (byte)updatedPosition));

                    if (HelperFunctions.GetByte(updatedPosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatedPosition -= 7;
                }
                updatedPosition = position - 9;
                while (updatedPosition >= 0 && updatedPosition % 8 != 7)
                {
                    if (HelperFunctions.GetByte(updatedPosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)updatedPosition), pieceType > 5 ? false : true))
                        moves.Add(new((byte)position, (byte)updatedPosition));
                    if (HelperFunctions.GetByte(updatedPosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatedPosition -= 9;
                }
            }
        }

        public void KingMovement(int pieceType, int position, ulong friendlyPieces, ref List<Move> moves, bool[] hostileAttackBoard, ChessBoard board)
        {
            if (HelperFunctions.GetByte(position, friendlyPieces) == 0)
                return;

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


                if (HelperFunctions.GetByte(target, friendlyPieces) == 0 && !hostileAttackBoard[target])
                {
                    if (blocksCheck(board, pieceType, new((byte)position, (byte)(target)), pieceType > 5 ? false : true)) moves.Add(new Move((byte)position, (byte)target));
                }
            }
        }

        public void Castle(bool IsWhite, ChessBoard board)
        {
            if (IsWhite)
            {
                if (board.WhiteCanCastleKingside)
                {
                    // its gonna be casteling here
                    Debug.Log("Castle");
                }
            }
        }

        public void EnPassant(ref ChessBoard board, byte startPosition, byte endPosition, int pieceType)
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

        public void CastelingRights(ref ChessBoard board, int pieceType, byte startPosition, byte endPosition)
        {
            // Remove all castling rights for white if king is moved
            if (pieceType == 5)
            {
                board.WhiteCanCastleKingside = false;
                board.WhiteCanCastleQueenside = false;
            }
            // Remove all castling rights for black if king is moved
            else if (pieceType == 11)
            {
                board.BlackCanCastleKingside = false;
                board.BlackCanCastleQueenside = false;
            }

            //Now white rooks
            else if (pieceType == 3)
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
            /*
            Debug.Log(board.WhiteCanCastleQueenside);
            Debug.Log(board.WhiteCanCastleKingside);
            Debug.Log(board.BlackCanCastleQueenside);
            Debug.Log(board.BlackCanCastleKingside);
            */
        }
        public void MakeAIMove(bool IsWhite)
        {

            Move move = ChessEngine.search.IterativeSearchAllMoves(ChessEngine.depth, IsWhite, ChessEngine.board);


            ChessEngine.board.UpdateBitBoards();
            ChessEngine.boardRenderer.UpdateBoard();
            int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, move.startPos, ChessEngine.board);

            Debug.Log("AI moving from " + move.startPos + " to " + move.endPos + " with a piece type of " + pieceType);

            ChessEngine.Mover.MovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType), pieceType, move, ref ChessEngine.board);
            ChessEngine.prevMove = move;

            /*Move[] moves = ChessEngine.search.SearchMoves(GetMovesForBlackOrWhite(IsWhite), false, ChessEngine.board);

            System.Random random = new System.Random();
            Move move = moves[random.Next(moves.Length)];

            int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, move.startPos, ChessEngine.board);


            ChessEngine.Mover.MovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType), pieceType, move.startPos, move.endPos);*/
        }

        public MovePieces.Move[] GetMovesForBlackOrWhite(bool IsWhite)
        {
            List<Move> moves = new();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int TEMPpieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, i * 8 + j);
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

        public void FiftyMoveRuleCounter(ref ChessBoard board, int pieceType, byte startPosition, byte endPosition)
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

        public Move[] GetMovesForBlackOrWhite(bool IsWhite, ChessBoard board)
        {
            Span<Move> moves = stackalloc Move[256]; 
            int counter = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int TEMPpieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, i * 8 + j, board);
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

        // first digit in vector is startY second is startX third is stopY fourth is stopX 


        public struct Move
        {
            public byte startPos;
            public byte endPos;

            public PromotionType promotionType;

            public Move(byte startPos1, byte endPos1)
            {
                startPos = startPos1;
                endPos = endPos1;
                promotionType = PromotionType.None;
            }

            public Move(byte _startPos, byte _endPos, PromotionType _promotionType)
            {
                startPos = _startPos;
                endPos = _endPos;
                promotionType = _promotionType;
            }
        }
    }
}