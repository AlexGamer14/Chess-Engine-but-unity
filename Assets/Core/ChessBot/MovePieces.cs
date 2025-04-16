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

            ChessEngine.MoveCount++;

            ChessEngine.board.WhiteToMove = !ChessEngine.board.WhiteToMove;

            pieces = pieces & ~(ulong)Math.Pow(2, startPosition);
            pieces = pieces | (ulong)Math.Pow(2, endPosition);

            ChessEngine.board.WhiteAttackBoard = new bool[64];

            ChessEngine.boardRenderer.UpdateBoard();
            ChessEngine.board.UpdateBitBoards();
        }


        public void CastleMover(ref ulong pieces, int pieceType, byte startPosition, byte endPosition, ChessBoard board)
        {

            board.EnPassantTargetSquare = 255;

            // I(Thomas) and Sondre think we should add 0.5 once. Debate this next time

            if (board.FiftyMoveRule >= 50)
            {
                Debug.Log("Draw by 50-Move rule");
            }
            Debug.Log(board.FiftyMoveRule);

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

            Debug.Log(board.WhiteCanCastleQueenside);
            Debug.Log(board.WhiteCanCastleKingside);
            Debug.Log(board.BlackCanCastleQueenside);
            Debug.Log(board.BlackCanCastleKingside);

            board.MoveCount += 0.5f;

            board.WhiteToMove = !board.WhiteToMove;

            pieces = pieces & ~(ulong)Math.Pow(2, startPosition);
            pieces = pieces | (ulong)Math.Pow(2, endPosition);

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
                            board.WhiteAttackBoard[position + 9] = true;
                            HelperFunctions.SetBit(ref board.WhiteAttackBitboard, position + 9, 1);
                            if (HelperFunctions.GetByte(position + 9, board.BlackPieces) == 1)
                            {
                                moves.Add(new(position, (byte)(position + 9)));

                            }
                        }
                        if (position % 8 != 0)
                        {
                            board.WhiteAttackBoard[position + 7] = true;
                            HelperFunctions.SetBit(ref board.WhiteAttackBitboard, position + 7, 1);

                            if (position % 8 != 0 && HelperFunctions.GetByte(position + 7, board.BlackPieces) == 1)
                            {
                                moves.Add(new(position, (byte)(position + 7)));
                            }
                        }

                    }
                    if (position % 8 != 7 && position + 9 == board.EnPassantTargetSquare)
                    {
                        moves.Add(new(position, (byte)(position + 9)));
                        board.WhiteAttackBoard[position + 9] = true;
                        HelperFunctions.SetBit(ref board.WhiteAttackBitboard, position + 9, 1);
                    }
                    if (position % 8 != 0 && position + 7 == board.EnPassantTargetSquare)
                    {
                        moves.Add(new(position, (byte)(position + 7)));
                        board.WhiteAttackBoard[position + 7] = true;
                        HelperFunctions.SetBit(ref board.WhiteAttackBitboard, position + 7, 1);
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
                        HelperFunctions.SetBit(ref board.WhiteAttackBitboard, position + 17, 1);
                        if (HelperFunctions.GetByte(position + 17, board.WhitePieces) != 1)
                        {
                            moves.Add(new(position, (byte)(position + 17)));
                        }
                    }
                    if (position % 8 != 0 && position < 48)
                    {
                        board.WhiteAttackBoard[position + 15] = true;
                        HelperFunctions.SetBit(ref board.WhiteAttackBitboard, position + 15, 1);
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
                            HelperFunctions.SetBit(ref board.WhiteAttackBitboard, position - 17, 1);
                            if (HelperFunctions.GetByte(position - 17, board.WhitePieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position - 17)));
                            }
                        }
                        if (position % 8 != 7)
                        {
                            board.WhiteAttackBoard[position - 15] = true;
                            HelperFunctions.SetBit(ref board.WhiteAttackBitboard, position - 15, 1);
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
                            HelperFunctions.SetBit(ref board.WhiteAttackBitboard, position + 10, 1);
                            if (HelperFunctions.GetByte(position + 10, board.WhitePieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position + 10)));
                            }
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            board.WhiteAttackBoard[position + 6] = true;
                            HelperFunctions.SetBit(ref board.WhiteAttackBitboard, position + 6, 1);
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
                                HelperFunctions.SetBit(ref board.WhiteAttackBitboard, position, 1);
                            }
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            if (HelperFunctions.GetByte(position - 10, board.WhitePieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position - 10)));
                                board.WhiteAttackBoard[position] = true;
                                HelperFunctions.SetBit(ref board.WhiteAttackBitboard, position, 1);
                            }
                        }
                    }
                    break;
                case 2:
                    BishopMovement(pieceType, position, board.WhitePieces, board.BlackPieces, ref moves, board, ref board.WhiteAttackBoard, ref board.WhiteAttackBitboard);
                    break;

                case 3:
                    RookMovement(pieceType, position, board.WhitePieces, board.BlackPieces, ref moves, board, ref board.WhiteAttackBoard, ref board.WhiteAttackBitboard);

                    break;

                case 4:
                    RookMovement(pieceType, position, board.WhitePieces, board.BlackPieces, ref moves, board, ref board.WhiteAttackBoard, ref board.WhiteAttackBitboard);
                    BishopMovement(pieceType, position, board.WhitePieces, board.BlackPieces, ref moves, board, ref board.WhiteAttackBoard, ref board.WhiteAttackBitboard);
                    break;
                case 5:

                    KingMovement(pieceType, position, board.WhitePieces, ref moves, ref board.WhiteAttackBoard, ref board.WhiteAttackBitboard);

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
                    if (position % 8 != 0 && position - 9 == board.EnPassantTargetSquare)
                    {
                        moves.Add(new(position, (byte)(position - 9)));
                    }
                    if (position % 8 != 7 && position - 7 == board.EnPassantTargetSquare)
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
                    BishopMovement(pieceType, position, board.BlackPieces, board.WhitePieces, ref moves, board, ref board.BlackAttackBoard, ref board.BlackAttackBitboard);
                    break;
                case 9:
                    RookMovement(pieceType, position, board.BlackPieces, board.WhitePieces, ref moves, board, ref board.BlackAttackBoard, ref board.BlackAttackBitboard);

                    break;
                case 10:
                    RookMovement(pieceType, position, board.BlackPieces, board.WhitePieces, ref moves, board, ref board.BlackAttackBoard, ref board.BlackAttackBitboard);
                    BishopMovement(pieceType, position, board.BlackPieces, board.WhitePieces, ref moves, board, ref board.BlackAttackBoard, ref board.BlackAttackBitboard);
                    break;
                case 11:
                    KingMovement(pieceType, position, board.BlackPieces, ref moves, ref board.BlackAttackBoard, ref board.BlackAttackBitboard);
                    break;

            }

            return moves.ToArray();
        }

        public void RookMovement(int pieceType, byte position, ulong friendlyPieces, ulong enemyPieces, ref List<Move> moves, ChessBoard board, ref bool[] AttackBoard, ref ulong AttackBitboard)
        {
            if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
            {
                return;
            }

            // Direction vectors for rook: up (-8), down (+8), right (+1), left (-1)
            int[] directions = { 8, -8, 1, -1 };

            foreach (int direction in directions)
            {
                int currentPosition = position;

                while (true)
                {
                    int targetPosition = currentPosition + direction;

                    // Check bounds
                    if (targetPosition < 0 || targetPosition >= 64)
                        break;

                    // Prevent wrapping from one rank to another on horizontal moves
                    if ((direction == 1 || direction == -1) &&
                        (currentPosition / 8 != targetPosition / 8))
                        break;

                    AttackBoard[targetPosition] = true;
                    HelperFunctions.SetBit(ref AttackBitboard, targetPosition, 1);

                    if (HelperFunctions.GetByte((byte)targetPosition, friendlyPieces) == 1)
                        break;

                    moves.Add(new Move(position, (byte)targetPosition));

                    if (HelperFunctions.GetByte((byte)targetPosition, enemyPieces) == 1)
                        break;

                    currentPosition = targetPosition;
                }
            }
        }



        public void BishopMovement(int pieceType, byte position, ulong friendlyPieces, ulong enemyPieces, ref List<Move> moves, ChessBoard board, ref bool[] AttackBoard, ref ulong AttackBitBoard)
        {
            if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType, ref board)) == 0)
            {
                return;
            }

            // Direction vectors for diagonals (top-right, top-left, bottom-right, bottom-left)
            var directions = new (int dx, int dy)[]
            {
        (1, 1),  // Top-right
        (-1, 1), // Top-left
        (1, -1), // Bottom-right
        (-1, -1) // Bottom-left
            };

            // Iterate over all four diagonal directions
            foreach (var (dx, dy) in directions)
            {
                int updatePosition = position;

                while (true)
                {
                    updatePosition += (byte)(dx + dy * 8); // Update position based on direction

                    // Check bounds: If we go out of bounds (wrap around), we stop.
                    if (updatePosition < 0 || updatePosition >= 64 || (dx == 1 && updatePosition % 8 == 0) || (dx == -1 && updatePosition % 8 == 7))
                    {
                        break;
                    }

                    // Mark the square as attacked
                    AttackBoard[updatePosition] = true;
                    HelperFunctions.SetBit(ref AttackBitBoard, updatePosition, 1);

                    // Check if the square is occupied by a friendly piece
                    if (HelperFunctions.GetByte(updatePosition, friendlyPieces) == 1)
                    {
                        break; // Stop if friendly piece is encountered
                    }

                    // Add the move if it's not blocked by a friendly piece
                    moves.Add(new(position, (byte)updatePosition));

                    // If the square is occupied by an enemy piece, stop the movement
                    if (HelperFunctions.GetByte(updatePosition, enemyPieces) == 1)
                    {
                        break; // Stop if enemy piece is encountered
                    }
                }
            }
        }


        public void KingMovement(int pieceType, byte position, ulong friendlyPieces, ref List<Move> moves, ref bool[] AttackBoard, ref ulong AttackBitboard)
        {
            if (HelperFunctions.GetByte(position, friendlyPieces) == 0)
            {
                return;
            }

            if (position < 56)
            {
                AttackBoard[position + 8] = true;
                HelperFunctions.SetBit(ref AttackBitboard, position + 8, 1);
                if (HelperFunctions.GetByte(position + 8, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position + 8)));
                }

                if (position < 55 && (position + 9) % 8 != 0)
                {
                    AttackBoard[position + 9] = true;
                    HelperFunctions.SetBit(ref AttackBitboard, position + 9, 1);
                }
                if (position < 55 && (position + 9) % 8 != 0 && HelperFunctions.GetByte(position + 9, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position + 9)));
                }

                if ((position + 7) % 8 != 7)
                {
                    AttackBoard[position + 7] = true;
                    HelperFunctions.SetBit(ref AttackBitboard, position + 7, 1);
                }

                if ((position + 7) % 8 != 7 && HelperFunctions.GetByte(position + 7, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position + 7)));
                }
            }

            if (position % 8 != 7)
            {
                AttackBoard[position + 1] = true;
                HelperFunctions.SetBit(ref AttackBitboard, position + 1, 1);
            }

            if (position % 8 != 0)
            {
                AttackBoard[position - 1] = true;
                HelperFunctions.SetBit(ref AttackBitboard, position - 1, 1);
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
                HelperFunctions.SetBit(ref AttackBitboard, position - 8, 1);

                if (HelperFunctions.GetByte(position - 8, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position - 8)));
                }

                if ((position % 8 != 0))
                {
                    AttackBoard[position - 9] = true;
                    HelperFunctions.SetBit(ref AttackBitboard, position - 9, 1);
                }

                if (position % 8 != 0 && HelperFunctions.GetByte(position - 9, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position - 9)));
                }

                if (position % 8 != 7)
                {
                    AttackBoard[position - 7] = true;
                    HelperFunctions.SetBit(ref AttackBitboard, position - 7, 1);
                }

                if (position % 8 != 7 && HelperFunctions.GetByte(position - 7, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position - 7)));
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
            Debug.Log(board.WhiteCanCastleQueenside);
            Debug.Log(board.WhiteCanCastleKingside);
            Debug.Log(board.BlackCanCastleQueenside);
            Debug.Log(board.BlackCanCastleKingside);
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
                if (endPosition == startPosition + 9 || endPosition == startPosition + 7)
                {
                    hasCaptured = CheckForCapture(pieceType, endPosition);
                }
            }

            else if (pieceType == 6)
            {
                if (endPosition == startPosition - 9 || endPosition == startPosition - 7)
                {
                    hasCaptured = CheckForCapture(pieceType, endPosition);
                }
            }

            else
            {
                hasCaptured = CheckForCapture(pieceType, endPosition);
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

        public bool KingInCheck(bool isWhite, ChessBoard board)
        {
            if (isWhite && (board.WhiteKing & board.BlackAttackBitboard) != 0)
            {
                return true;
            }

            if (!isWhite && (board.BlackKing & board.WhiteAttackBitboard) != 0)
            {
                return true;
            }

            return false;
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

