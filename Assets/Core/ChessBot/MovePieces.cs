using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ChessEngine
{
    public class MovePieces
    {
        public void MovePiece(ref ulong pieces, int pieceType, byte startPosition, byte endPosition)
        {

            if (pieceType == 0 && startPosition + 16 == endPosition)
            {
                ChessEngine.MovedTwoSpacesLastTurn = endPosition;
            }
            else if (pieceType == 6 && startPosition - 16 == endPosition)
            {
                ChessEngine.MovedTwoSpacesLastTurn = endPosition;
            }
            else 
            {
                ChessEngine.MovedTwoSpacesLastTurn = byte.MaxValue;
            }



            CheckForCapture(pieceType, endPosition);

            ChessEngine.WhiteToMove = !ChessEngine.WhiteToMove;

            pieces = pieces & ~(ulong)Math.Pow(2, startPosition);
            pieces = pieces | (ulong)Math.Pow(2, endPosition);
            ChessEngine.boardRenderer.UpdateBoard();
            ChessEngine.board.UpdateBitBoards();
        }

        public void SearchMovePiece(ref ulong pieces, int pieceType, byte startPosition, byte endPosition, ref ChessBoard board)
        {
            CheckForCapture(pieceType, endPosition);

            pieces = pieces & ~(ulong)Math.Pow(2, startPosition);
            pieces = pieces | (ulong)Math.Pow(2, endPosition);
            board.UpdateBitBoards();
        }

        public void CheckForCapture(int pieceType, int pos)
        {
            int pieceTypeCheck = HelperFunctions.CheckIfPieceOnEveryBoard(pieceType, pos);
            if (pieceTypeCheck != int.MaxValue)
            {
                HelperFunctions.SetBit(ref HelperFunctions.GetTypeBasedOnIndex(pieceTypeCheck), pos);
            }
        }
        public void CheckForCapture(int pieceType, int pos, ref ChessBoard board)
        {
            int pieceTypeCheck = HelperFunctions.CheckIfPieceOnEveryBoard(pieceType, pos, board);
            if (pieceTypeCheck != int.MaxValue)
            {
                HelperFunctions.SetBit(ref HelperFunctions.GetTypeBasedOnIndex(pieceTypeCheck, ref board), pos);
            }
        }

        public bool LegalMove(byte pieceType, byte startPosition, byte endPosition)
        {    
            // 0 = White Pawn, 1 = White Knigth, 2 = White Bishop, 3 = White Rook, 4 = White Queen, 5 = White King
            // 6 = Black Pawn, 7 = Black Knigth, 8 = Black Bishop, 9 = Black Rook, 10 = Black Queen, 11 = Black King
            switch (pieceType) 
            {
                case 1:
                    if (startPosition == 6 && endPosition == 7)
                    {

                    }
                    break;

                case 6:
                    if (startPosition == 6)
                    {

                    }
                    break;
            }
            return false;
        }


        public Move CheckForCastle(bool IsWhite)
        {
            if (IsWhite && !ChessEngine.HasWhiteKingMoved)
            {
                return new();
            }
            return new();
        }

        public Move[] GetLegalMoves(ChessBoard board ,byte pieceType,byte position)
        {
            List<Move> moves = new();
            UInt64 whiteAttackBoard = 0;
            // UInt64 blackAttackBoard = 0;

            switch (pieceType)
            {
                case 0:
                    if (HelperFunctions.GetByte(position,HelperFunctions.GetTypeBasedOnIndex(pieceType))==0)
                    {
                        return moves.ToArray();
                    }

                    if (position < 56 && HelperFunctions.GetByte(position + 8, board.AllPieces) == 0)
                    {
                        moves.Add(new Move(position, (byte)(position + 8)));
                        whiteAttackBoard = (ulong)(whiteAttackBoard + Math.Pow(position+8, 2));
                        if (position > 7 && position < 16 && position < 48 && HelperFunctions.GetByte(position + 16, board.AllPieces) == 0)
                        {
                            moves.Add(new(position, (byte)(position + 16)));
                            whiteAttackBoard = (ulong)(whiteAttackBoard + Math.Pow(position+16, 2));
                        }
                    }
                    if (position < 56)
                    {
                        if (position%8!=7 && HelperFunctions.GetByte(position+9, board.BlackPieces) == 1)
                        {
                            moves.Add(new(position, (byte)(position + 9)));
                            whiteAttackBoard = (ulong)(whiteAttackBoard + Math.Pow(position+9, 2));
                        }
                        if (position%8!=0 && HelperFunctions.GetByte(position+7, board.BlackPieces) == 1)
                        {
                            moves.Add(new(position, (byte)(position +7 )));
                            whiteAttackBoard = (ulong)(whiteAttackBoard + Math.Pow(position+7, 2));
                        }
                    }
                    break;
                case 1:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType)) == 0)
                    {
                        return moves.ToArray();
                    }
                    if (position % 8 != 7 && position < 48)
                    {
                        if (HelperFunctions.GetByte(position + 17, board.WhitePieces) != 1)
                        {
                            moves.Add(new(position, (byte)(position + 17)));
                            whiteAttackBoard = (ulong)(whiteAttackBoard + Math.Pow(position+17, 2));
                        }
                    }
                    if (position%8!=0&&position<48)
                    {
                        if (HelperFunctions.GetByte(position + 15, board.WhitePieces) != 1)
                        {
                            moves.Add(new(position, (byte)(position + 15)));
                            whiteAttackBoard = (ulong)(whiteAttackBoard + Math.Pow(position+15, 2));
                        }
                    }

                    if (position>15)
                    {
                        if (position%8!=0)
                        {
                            if (HelperFunctions.GetByte(position - 17, board.WhitePieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position - 17)));
                                whiteAttackBoard = (ulong)(whiteAttackBoard + Math.Pow(position-17, 2));
                            }
                        }
                        if (position%8!=7) 
                        {
                            if (HelperFunctions.GetByte(position - 15, board.WhitePieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position - 15)));
                                whiteAttackBoard = (ulong)(whiteAttackBoard + Math.Pow(position - 17, 2));
                            }
                        }
                    }

                    if (position < 56)
                    {
                        if (position % 8 != 7 && position % 8 != 6)
                        {
                            if (HelperFunctions.GetByte(position + 10, board.WhitePieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position + 10)));
                                whiteAttackBoard = (ulong)(whiteAttackBoard + Math.Pow(position + 10, 2));
                            }
                        }
                        if (position%8!=0&&position%8!=1)
                        {
                            if (HelperFunctions.GetByte(position + 6, board.WhitePieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position + 6)));
                                whiteAttackBoard = (ulong)(whiteAttackBoard + Math.Pow(position + 6, 2));
                            }
                        }
                    }

                    if (position>7)
                    {
                        if (position % 8 != 7 && position % 8 != 6)
                        {
                            if (HelperFunctions.GetByte(position - 6, board.WhitePieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position - 6)));
                                whiteAttackBoard = (ulong)(whiteAttackBoard + Math.Pow(position - 6, 2));
                            }
                        }
                        if (position % 8 != 0 && position % 8 != 1)
                        {
                            if (HelperFunctions.GetByte(position - 10, board.WhitePieces) != 1)
                            {
                                moves.Add(new(position, (byte)(position - 10)));
                                whiteAttackBoard = (ulong)(whiteAttackBoard + Math.Pow(position-10, 2));
                            }
                        }
                    }
                    break;
                case 2:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType)) == 0)
                    {
                        return moves.ToArray();
                    }
                    if (position < 56)
                    {
                        int updatePosition = position + 9;
                        while (updatePosition < 64 && updatePosition %8!=0)
                        {
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            moves.Add(new(position, (byte)(updatePosition)));
                            if (HelperFunctions.GetByte(updatePosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatePosition += 9;

                        }
                        updatePosition = position + 7;
                        while (updatePosition < 63 && updatePosition % 8 != 7)
                        {
                            if (HelperFunctions.GetByte(updatePosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            moves.Add(new(position, (byte)(updatePosition)));
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
                            if (HelperFunctions.GetByte(updatedPosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            moves.Add(new(position, (byte)updatedPosition));
                            if (HelperFunctions.GetByte(updatedPosition, board.BlackPieces) == 1) 
                            {
                                break;
                            }
                            updatedPosition -= 7;
                        }
                        updatedPosition = position - 9;
                        while (updatedPosition >= 0 && updatedPosition % 8 != 7)
                        {
                            if (HelperFunctions.GetByte(updatedPosition, board.WhitePieces) == 1)
                            {
                                break;
                            }
                            moves.Add(new(position, (byte)updatedPosition));
                            if (HelperFunctions.GetByte(updatedPosition, board.BlackPieces) == 1)
                            {
                                break;
                            }
                            updatedPosition -= 9;
                        }
                    }
                    
                    break;

                case 3:
                    RookMovement(pieceType, position, board.WhitePieces, board.BlackPieces, ref moves);

                    break;

                case 4:
                    RookMovement(pieceType,position,board.WhitePieces, board.BlackPieces,ref moves);
                    BishopMovement(pieceType, position, board.WhitePieces, board.BlackPieces, ref moves);
                    break;
                case 5:

                    KingMovement(pieceType, position, board.WhitePieces, ref moves);
                    ChessEngine.HasWhiteKingMoved = true;

                    break;
                case 6:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType)) == 0)
                    {
                        return moves.ToArray();
                    }

                    if (position > 7 && HelperFunctions.GetByte(position-8, board.AllPieces) == 0)
                    {
                        moves.Add(new Move(position, (byte)(position - 8)));
                        if (position > 47 && position < 56 && position > 15 &&HelperFunctions.GetByte(position-16, board.AllPieces) == 0)
                        {
                            moves.Add( new(position, (byte)(position - 16)));
                        }
                    }

                    if (position > 7)
                    {
                        if (position%8!=7 && HelperFunctions.GetByte(position-7, board.WhitePieces) == 1)
                        {
                            moves.Add(new(position, (byte)(position - 7)));
                        }
                        if (position%8!=0 && HelperFunctions.GetByte(position-9, board.WhitePieces) == 1)
                        {
                            moves.Add(new(position, (byte)(position - 9)));
                        }
                    }
                    break;
                case 7:
                    if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType)) == 0)
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
                    BishopMovement(pieceType, position, board.BlackPieces, board.WhitePieces, ref moves);
                    break;
                case 9:
                    RookMovement(pieceType, position, board.BlackPieces, board.WhitePieces, ref moves);

                    break;
                case 10:
                    RookMovement(pieceType,position,board.BlackPieces,board.WhitePieces,ref moves);
                    BishopMovement(pieceType, position, board.BlackPieces, board.WhitePieces, ref moves);
                    break;
                case 11:
                    KingMovement(pieceType,position, board.BlackPieces,ref moves);
                    ChessEngine.HasBlackKingMoved = true;
                    break;
                
            }

            return moves.ToArray();
        }

        public void RookMovement(int pieceType, byte position, ulong friendlyPieces, ulong enemyPieces, ref List<Move> moves)
        {
            if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType)) == 0)
            {
                return;
            }
            if (position < 56)
            {
                byte updatePosition = (byte)(position + 8);
                while (updatePosition - 8 < 56)
                {
                    if (HelperFunctions.GetByte(updatePosition, friendlyPieces) == 1)
                    {
                        break;
                    }
                    moves.Add(new(position, updatePosition));
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

            if (position%8!=0)
            {
                int updatePosition = position - 1;
                while (updatePosition % 8 != 7)
                {
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

        public void BishopMovement(int pieceType, byte position, ulong friendlyPieces, ulong enemyPieces, ref List<Move> moves)
        {
            if (HelperFunctions.GetByte(position, HelperFunctions.GetTypeBasedOnIndex(pieceType)) == 0)
            {
                return;
            }
            if (position < 56)
            {
                int updatePosition = position + 9;
                while (updatePosition < 64 && updatePosition % 8 != 0)
                {
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

        public void KingMovement(int pieceType, byte position, ulong friendlyPieces, ref List<Move> moves)
        {
            if (HelperFunctions.GetByte(position,friendlyPieces) == 0)
            {
                return;
            }

            if (position < 56)
            {
                if (HelperFunctions.GetByte(position + 8, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position + 8)));
                }

                if (position < 55 && (position + 9) % 8 != 0 && HelperFunctions.GetByte(position + 9, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position + 9)));
                }
                if ((position + 7) % 8 != 7 && HelperFunctions.GetByte(position + 7, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position + 7)));
                }
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
                if (HelperFunctions.GetByte(position - 8, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position - 8)));
                }

                if (position % 8 != 0 && HelperFunctions.GetByte(position - 9, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position - 9)));
                }
                if (position % 8 != 7 && HelperFunctions.GetByte(position - 7, friendlyPieces) == 0)
                {
                    moves.Add(new(position, (byte)(position - 7)));
                }
            }
        }

        public void MakeAIMove(bool IsWhite)
        {
            Move move = ChessEngine.search.RefactoredSearchAllMoves(3, IsWhite, ChessEngine.board);

            int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, move.startPos, ChessEngine.board);


            ChessEngine.Mover.MovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType), pieceType, move.startPos, move.endPos);


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

                    Move[] legalMoves = GetLegalMoves(ChessEngine.board, (byte)TEMPpieceType, (byte)(i * 8 + j));

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

                    Move[] legalMoves = GetLegalMoves(board, (byte)TEMPpieceType, (byte)(i * 8 + j));

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

        public struct Move
        {
            public byte startPos;
            public byte endPos;

            public Move(byte startPos1, byte endPos1)
            {
                startPos= startPos1;
                endPos= endPos1;
            }
        }
    }
}