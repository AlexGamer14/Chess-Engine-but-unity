using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

namespace ChessEngine
{
    public class MovePieces
    {
        public void MovePiece(ref ulong pieces, int pieceType, byte startPosition, byte endPosition, ChessBoard board)
        {

            EnPassant(board, startPosition, endPosition, pieceType);
            FiftyMoveRuleCounter(board, pieceType, startPosition, endPosition);
            CastelingRights(board, pieceType, startPosition, endPosition);

            board.MoveCount++;

            board.WhiteToMove = !board.WhiteToMove;

            pieces = pieces & ~(ulong)Math.Pow(2, startPosition);
            pieces = pieces | (ulong)Math.Pow(2, endPosition);

            board.WhiteAttackBoard = new bool[64];

            ChessEngine.boardRenderer.UpdateBoard();
            board.UpdateBitBoards();
        }

        
        public void CastleMover(ref ulong pieces, int pieceType, byte startPosition, byte endPosition, ChessBoard board)
        {
            
            board.EnPassantTargetSquare = 255;
            
            // I(Thomas) and Sondre think we should add 0.5 once. Debate this next time
            board.FiftyMoveRule += (float)0.25;

            if(board.FiftyMoveRule >= 50)
            {
                Debug.Log("Draw by 50-Move rule");
            }
            Debug.Log(board.FiftyMoveRule);

            // Remove all castling rights for white if king is moved
            if(pieceType == 5)
            {
                board.WhiteCanCastleKingside = false;
                board.WhiteCanCastleQueenside = false;
            }
            // Remove all castling rights for black if king is moved
            else if(pieceType == 11)
            {
                board.BlackCanCastleKingside = false;
                board.BlackCanCastleQueenside = false;
            }

            /*
            Debug.Log(board.WhiteCanCastleQueenside);
            Debug.Log(board.WhiteCanCastleKingside);
            Debug.Log(board.BlackCanCastleQueenside);
            Debug.Log(board.BlackCanCastleKingside);
            */
         
            board.MoveCount+=0.5f;

            board.WhiteToMove = !board.WhiteToMove;

            pieces = pieces & ~(ulong)Math.Pow(2, startPosition);
            pieces = pieces | (ulong)Math.Pow(2, endPosition);

            board.WhiteAttackBoard = new bool[64];
            board.BlackAttackBoard = new bool[64];

            board.UpdateBitBoards();
        }


        public void SearchMovePiece(ref ulong pieces, int pieceType, byte startPosition, byte endPosition, ref ChessBoard board)
        {
            if (pieceType == 0)
            {
                if (endPosition == startPosition + 16)
                {
                    board.EnPassantTargetSquare = (byte)(endPosition - (byte)8);
                }
                if (endPosition == board.EnPassantTargetSquare)
                {
                    CheckForCapture(pieceType, endPosition - 8, ref board);
                    board.EnPassantTargetSquare = 255;
                }
                else
                {
                    board.EnPassantTargetSquare = 255;
                }
            }
            if (pieceType == 6)
            {
                if (endPosition == startPosition - 16)
                {
                    board.EnPassantTargetSquare = (byte)(startPosition + (byte)8);
                }
                if (endPosition == board.EnPassantTargetSquare)
                {
                    CheckForCapture(pieceType, endPosition + 8);
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
            
            /*if(pieceType != 0 || pieceType != 6)
            {
                if (!hasCaptured)
                {
                    ChessEngine.board.FiftyMoveRule += (float)0.5;
                }
            }
            else
            {
                ChessEngine.board.FiftyMoveRule = 0;
            }

            if(ChessEngine.board.FiftyMoveRule <= 50)
            {
                Debug.Log("Draw by 50-Move rule");
            }
            Debug.Log(ChessEngine.board.FiftyMoveRule);*/

            CheckForCapture(pieceType, endPosition, ref board);

            pieces = pieces & ~(ulong)Math.Pow(2, startPosition);
            pieces = pieces | (ulong)Math.Pow(2, endPosition);
            board.UpdateBitBoards();
        }

        public bool CheckForCapture(int pieceType, int pos)
        {
            int pieceTypeCheck = HelperFunctions.CheckIfPieceOnEveryBoard(pieceType, pos);
            if (pieceTypeCheck != int.MaxValue)
            {
                HelperFunctions.SetBit(ref HelperFunctions.GetTypeBasedOnIndex(pieceTypeCheck), pos);
                return true;
            }
            return false;
            
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

        public Move[] GetLegalMoves(ref ChessBoard board, byte pieceType, byte position)
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
                        moves.Add(new Move(position, (byte)(position + 8)));
                        
                        if (position > 7 && position < 16 && position < 48 && HelperFunctions.GetByte(position + 16, board.AllPieces) == 0)
                        {
                            moves.Add(new(position, (byte)(position + 16)));
                           
                        }
                    }
                    if (position < 56)
                    {
                        if (position % 8 != 7)
                        {
                            board.WhiteAttackBoard[position+9] = true;
                            if (HelperFunctions.GetByte(position + 9, board.BlackPieces) == 1)
                            {
                                moves.Add(new(position, (byte)(position + 9)));

                            }
                        }
                        if (position % 8 != 0)
                        {
                            board.WhiteAttackBoard[position+7]=true;

                            if (position % 8 != 0 && HelperFunctions.GetByte(position + 7, board.BlackPieces) == 1)
                            {
                                moves.Add(new(position, (byte)(position + 7)));
                            }
                        }
                        
                    }
                    if (position % 8 != 7 && position + 9 == board.EnPassantTargetSquare)
                    {
                        moves.Add(new(position, (byte)(position + 9)));
                        board.WhiteAttackBoard[position+9] = true;
                    }
                    if (position % 8 != 0 && position + 7 == board.EnPassantTargetSquare)
                    {
                        moves.Add(new(position, (byte)(position + 7)));
                        board.WhiteAttackBoard[position+7] = true;
                    }
                        
                    break;
                case 1:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        return moves.ToArray();
                    }
                    if (position % 8 != 7 && position < 48)
                    {
                        board.WhiteAttackBoard[position + 17] = true;
                        if (HelperFunctions.GetByte(position + 17, board.WhitePieces) != 1)
                        {
                            moves.Add(new(position, (byte)(position + 17)));
                        }
                    }
                    if (position % 8 != 0 && position < 48)
                    {
                        board.WhiteAttackBoard[position + 15] = true;
                        if (HelperFunctions.GetByte(position + 15, board.WhitePieces) != 1)
                        {
                            moves.Add(new(position, (byte)(position + 15)));
                        }
                    }

                    if (position > 15)
                    {
                        if (position % 8 != 0)
                        {
                            board.WhiteAttackBoard[position - 17] = true;
                            if (HelperFunctions.GetByte(position - 17, board.WhitePieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position - 17)));
                            }
                        }
                        if (position % 8 != 7)
                        {
                            board.WhiteAttackBoard[position - 15] = true;
                            if (HelperFunctions.GetByte(position - 15, board.WhitePieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position - 15)));
                            }
                        }
                    }

                    if (position < 56)
                    {
                        if (position % 8 != 7 && position % 8 != 6)
                        {
                            board.WhiteAttackBoard[position + 10] = true;
                            if (HelperFunctions.GetByte(position + 10, board.WhitePieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position + 10)));
                            }
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            board.WhiteAttackBoard[position + 6] = true;
                            if (HelperFunctions.GetByte(position + 6, board.WhitePieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position + 6)));
                            }
                        }
                    }

                    if (position > 7)
                    {
                        if (position % 8 != 7 && position % 8 != 6)
                        {
                            if (HelperFunctions.GetByte(position - 6, board.WhitePieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position - 6)));
                                board.WhiteAttackBoard[position] = true;
                            }
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            if (HelperFunctions.GetByte(position - 10, board.WhitePieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position - 10)));
                                board.WhiteAttackBoard[position] = true;
                            }
                        }
                    }
                    break;
                case 2:
                    BishopMovement(pieceType, position, board.WhitePieces, board.BlackPieces, ref moves, board, ref board.WhiteAttackBoard);
                    break;

                case 3:
                    RookMovement(pieceType, position, board.WhitePieces, board.BlackPieces, ref moves, board, ref board.WhiteAttackBoard);

                    break;

                case 4:
                    RookMovement(pieceType, position, board.WhitePieces, board.BlackPieces, ref moves, board, ref board.WhiteAttackBoard);
                    BishopMovement(pieceType, position, board.WhitePieces, board.BlackPieces, ref moves, board, ref board.WhiteAttackBoard);
                    break;
                case 5:

                    KingMovement(pieceType, position, board.WhitePieces, ref moves, ref board.WhiteAttackBoard);

                    break;
                case 6:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
                    {
                        return moves.ToArray();
                    }

                    if (position > 7 && HelperFunctions.GetByte(position - 8, board.AllPieces) == 0)
                    {
                        moves.Add(new Move(position, (byte)(position - 8)));
                        if (position > 47 && position < 56 && position > 15 && HelperFunctions.GetByte(position - 16, board.AllPieces) == 0)
                        {
                            moves.Add(new(position, (byte)(position - 16)));
                        }
                    }

                    if (position > 7)
                    {
                        if (position % 8 != 7 && HelperFunctions.GetByte(position - 7, board.WhitePieces) == 1)
                        {
                            moves.Add(new(position, (byte)(position - 7)));
                        }
                        if (position % 8 != 0 && HelperFunctions.GetByte(position - 9, board.WhitePieces) == 1)
                        {
                            moves.Add(new(position, (byte)(position - 9)));
                        }
                    }
                    if (position%8!=0 && position - 9 == board.EnPassantTargetSquare)
                    {
                        moves.Add(new(position, (byte)(position - 9)));
                    }
                    if (position%8!=7 &&  position - 7 == board.EnPassantTargetSquare)
                    {
                        moves.Add(new(position, (byte)(position - 7)));
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
                            moves.Add(new(position, (byte)(position + 17)));
                        }
                    }
                    if (position % 8 != 0 && position < 48)
                    {
                        if (HelperFunctions.GetByte(position + 15, board.BlackPieces) != 1)
                        {
                            moves.Add(new(position, (byte)(position + 15)));
                        }
                    }

                    if (position > 15)
                    {
                        if (position % 8 != 0)
                        {
                            if (HelperFunctions.GetByte(position - 17, board.BlackPieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position - 17)));
                            }
                        }
                        if (position % 8 != 7)
                        {
                            if (HelperFunctions.GetByte(position - 15, board.BlackPieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position - 15)));
                            }
                        }
                    }

                    if (position < 56)
                    {
                        if (position % 8 != 7 && position % 8 != 6)
                        {
                            if (HelperFunctions.GetByte(position + 10, board.BlackPieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position + 10)));
                            }
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            if (HelperFunctions.GetByte(position + 6, board.BlackPieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position + 6)));
                            }
                        }
                    }

                    if (position > 7)
                    {
                        if (position % 8 != 7 && position % 8 != 6)
                        {
                            if (HelperFunctions.GetByte(position - 6, board.BlackPieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position - 6)));
                            }
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            if (HelperFunctions.GetByte(position - 10, board.BlackPieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position - 10)));
                            }
                        }
                    }
                    break;
                case 8:
                    BishopMovement(pieceType, position, board.BlackPieces, board.WhitePieces, ref moves, board, ref board.BlackAttackBoard);
                    break;
                case 9:
                    RookMovement(pieceType, position, board.BlackPieces, board.WhitePieces, ref moves, board, ref board.BlackAttackBoard);

                    break;
                case 10:
                    RookMovement(pieceType, position, board.BlackPieces, board.WhitePieces, ref moves, board, ref board.BlackAttackBoard);
                    BishopMovement(pieceType, position, board.BlackPieces, board.WhitePieces, ref moves, board, ref board.BlackAttackBoard);
                    break;
                case 11:
                    KingMovement(pieceType, position, board.BlackPieces, ref moves, ref board.BlackAttackBoard);
                    break;

            }

            return moves.ToArray();
        }

        public void RookMovement(int pieceType, byte position, ulong friendlyPieces, ulong enemyPieces, ref List<Move> moves, ChessBoard board, ref bool[] AttackBoard)
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
                    AttackBoard[updatePosition] = true;
                    if (HelperFunctions.GetByte(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    moves.Add(new(position, (byte)updatePosition));
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
                    AttackBoard[updatePosition] = true;
                    if (HelperFunctions.GetByte(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    moves.Add(new(position, (byte)updatePosition));
                    if (HelperFunctions.GetByte(updatePosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatePosition -= 8;
                }
            }

            if (position % 8 != 7)
            {
                byte updatePosition = (byte)(position + 1);
                while (updatePosition % 8 != 0)
                {
                    AttackBoard[updatePosition] = true;
                    if (HelperFunctions.GetByte(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    moves.Add(new(position, updatePosition));
                    if (HelperFunctions.GetByte(updatePosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatePosition++;
                }
            }

            if (position % 8 != 0)
            {
                byte updatePosition = (byte)(position - 1);
                while (updatePosition % 8 != 7)
                {
                    AttackBoard[updatePosition] = true;
                    if (HelperFunctions.GetByte(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    moves.Add(new(position, (byte)(updatePosition)));
                    if (HelperFunctions.GetByte(updatePosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatePosition--;
                }
            }
        }

        public void BishopMovement(int pieceType, byte position, ulong friendlyPieces, ulong enemyPieces, ref List<Move> moves, ChessBoard board, ref bool[] AttackBoard)
        {
            if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
            {
                return;
            }
            if (position < 56)
            {
                int updatePosition = position + 9;
                while (updatePosition < 64 && updatePosition % 8 != 0)
                {
                    AttackBoard[updatePosition] = true;
                    if (HelperFunctions.GetByte(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    moves.Add(new(position, (byte)(updatePosition)));
                    if (HelperFunctions.GetByte(updatePosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatePosition += 9;

                }
                updatePosition = position + 7;
                while (updatePosition < 63 && updatePosition % 8 != 7)
                {
                    AttackBoard[updatePosition] = true;
                    if (HelperFunctions.GetByte(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    moves.Add(new(position, (byte)(updatePosition)));
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
                    AttackBoard[updatedPosition] = true;
                    if (HelperFunctions.GetByte(updatedPosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    moves.Add(new(position, (byte)updatedPosition));

                    if (HelperFunctions.GetByte(updatedPosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatedPosition -= 7;
                }
                updatedPosition = position - 9;
                while (updatedPosition >= 0 && updatedPosition % 8 != 7)
                {
                    AttackBoard[updatedPosition] = true;
                    if (HelperFunctions.GetByte(updatedPosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    moves.Add(new(position, (byte)updatedPosition));
                    if (HelperFunctions.GetByte(updatedPosition, enemyPieces) == 1)
                    {
                        break;
                    }
                    updatedPosition -= 9;
                }
            }
        }

        public void KingMovement(int pieceType, byte position, ulong friendlyPieces, ref List<Move> moves, ref bool[] AttackBoard)
        {
            if (HelperFunctions.GetByte(position, friendlyPieces) == 0)
            {
                return;
            }

            if (position < 56)
            {
                AttackBoard[position + 8] = true;
                if (HelperFunctions.GetByte(position + 8, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position + 8)));
                }

                if (position < 55 && (position + 9) % 8 != 0)
                {
                    AttackBoard[position + 9] = true;
                }
                if (position < 55 && (position + 9) % 8 != 0 && HelperFunctions.GetByte(position + 9, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position + 9)));
                }

                if ((position + 7) % 8 != 7)
                {
                    AttackBoard[position + 7] = true;
                }

                if ((position + 7) % 8 != 7 && HelperFunctions.GetByte(position + 7, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position + 7)));
                }
            }

            if (position % 8 != 7)
            {
                AttackBoard[position + 1] = true;
            }

            if (position % 8 != 0)
            {
                AttackBoard[position - 1] = true;
            }

            if (position % 8 != 7 && HelperFunctions.GetByte(position + 1, friendlyPieces) == 0)
            {
                moves.Add(new(position, (byte)(position + 1)));
            }
            if (position % 8 != 0 && HelperFunctions.GetByte(position - 1, friendlyPieces) == 0)
            {
                moves.Add(new(position, (byte)(position - 1)));
            }

            if (position > 7)
            {
                AttackBoard[position - 8] = true;

                if (HelperFunctions.GetByte(position - 8, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position - 8)));
                }

                if ((position % 8 != 0))
                {
                    AttackBoard[position - 9] = true;
                }

                if (position % 8 != 0 && HelperFunctions.GetByte(position - 9, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position - 9)));
                }

                if (position % 8 != 7 )
                {
                    AttackBoard[position - 7] = true;
                }

                if (position % 8 != 7 && HelperFunctions.GetByte(position - 7, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position - 7)));
                }
            }
        }

        public void Castle(bool IsWhite, ChessBoard board)
        {
            if(IsWhite)
            {
                if (board.WhiteCanCastleKingside)
                {
                    // its gonna be casteling here
                    Debug.Log("Castle");
                }
            }
        }

        public void EnPassant(ChessBoard board, byte startPosition, byte endPosition, int pieceType)
        {
            // Value set to 255 so that it is not on board if en passant is not possible

            if (pieceType == 0)
            {
                
                if (endPosition == board.EnPassantTargetSquare)
                {
                    CheckForCapture(pieceType, endPosition - 8);
                    board.EnPassantTargetSquare = 255;
                }

                Debug.Log(pieceType);
                Debug.Log(startPosition);
                Debug.Log(endPosition);
                

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
                    CheckForCapture(pieceType, endPosition + 8);
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

        public void CastelingRights(ChessBoard board, int pieceType, byte startPosition, byte endPosition)
        {
            // Remove all castling rights for white if king is moved
            if(pieceType == 5)
            {
                board.WhiteCanCastleKingside = false;
                board.WhiteCanCastleQueenside = false;
            }
            // Remove all castling rights for black if king is moved
            else if(pieceType == 11)
            {
                board.BlackCanCastleKingside = false;
                board.BlackCanCastleQueenside = false;
            }
            
            //Now white rooks
            else if(pieceType == 3)
            {
                if(startPosition == 0)
                {
                    board.WhiteCanCastleQueenside = false;
                }
                if(startPosition == 7)
                {
                    board.WhiteCanCastleKingside = false;
                } 
            }

            //Now black rooks
            else if(pieceType == 9)
            {
                if(startPosition == 56)
                {
                    board.BlackCanCastleQueenside = false;
                }
                if(startPosition == 63)
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

            Move move = ChessEngine.search.IterativeSearchAllMoves(3, IsWhite, ChessEngine.board);


            ChessEngine.board.UpdateBitBoards();
            ChessEngine.boardRenderer.UpdateBoard();
            int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, move.startPos, ChessEngine.board);

            Debug.Log("AI moving from " + move.startPos + " to " + move.endPos + " with a piece type of " + pieceType);


            ChessEngine.Mover.MovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType), pieceType, move.startPos, move.endPos, ChessEngine.board);


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

        public void FiftyMoveRuleCounter(ChessBoard board, int pieceType, byte startPosition, byte endPosition)
        {
            bool hasCaptured = false;

            if (pieceType == 0)
            {
                if (endPosition==startPosition+9 || endPosition==startPosition+7)
                {
                    hasCaptured= CheckForCapture(pieceType, endPosition);
                }
            }

            else if (pieceType==6)
            {
                if (endPosition==startPosition-9 || endPosition==startPosition-7)
                {
                    hasCaptured= CheckForCapture(pieceType, endPosition);
                }
            }

            else
            {
                hasCaptured = CheckForCapture(pieceType, endPosition);
            }
            
            if(pieceType != 0 && pieceType != 6)
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

            if(board.FiftyMoveRule >= 50)
            {
                Debug.Log("Draw by 50-Move rule");
            }
            Debug.Log(board.FiftyMoveRule);
        }

        public MovePieces.Move[] GetMovesForBlackOrWhite(bool IsWhite, ChessBoard board)
        {
            List<Move> moves = new();

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
                        moves.Add(legalMoves[z]);
                    }
                }
            }
            return moves.ToArray();
        }
        
        // first digit in vector is startY second is startX third is stopY fourth is stopX 
        

        public struct Move
        {
            public byte startPos;
            public byte endPos;

            public Move(byte startPos1, byte endPos1)
            {
                startPos = startPos1;
                endPos = endPos1;
            }
        }
    }
}

