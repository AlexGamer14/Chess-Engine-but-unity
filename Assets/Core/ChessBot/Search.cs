using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace ChessEngine
{
    public class Search
    {
        public MovePieces.Move SearchAllMoves(int depth, bool WhiteToMove)
        {
            MovePieces.Move[][] moves = ChessEngine.Mover.GetMovesForBlackOrWhite(false);

            MovePieces.Move[] bestMoves = SearchMoves(moves, false);

            float[] bestWhiteMoveEval = new float[bestMoves.Length];

            for (int i = 0; i < bestMoves.Length; i++)
            {
                ChessEngine.boardRenderer.UpdateBoard();

                ChessBoard testBoard = (ChessBoard)ChessEngine.board.Clone();
                
                for (int j = 0; j < 8; j++)
                {
                    Debug.Log(HelperFunctions.GetByte(j+48, testBoard.BlackPawns));
                }
                
                Debug.Log(bestMoves[i].startPos);
                int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, bestMoves[i].startPos, testBoard);
                ulong pieces = HelperFunctions.GetTypeBasedOnIndex(pieceType, ref testBoard);

                ChessEngine.Mover.SearchMovePiece(ref pieces, pieceType, bestMoves[i].startPos, bestMoves[i].endPos, ref testBoard);

                // very good variable name ik
                List<MovePieces.Move[]> newMoves = new();

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        int TEMPpieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, y * 8 + x, testBoard);
                        if (TEMPpieceType == int.MaxValue)
                        {
                            continue;
                        }
                        if (WhiteToMove)
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

                        MovePieces.Move[] legalMoves = ChessEngine.Mover.GetLegalMoves(testBoard, (byte)TEMPpieceType, (byte)(y * 8 + x));

                        if (legalMoves.Length <= 0)
                        {
                            continue;
                        }


                        newMoves.Add(legalMoves);
                    }
                }

                MovePieces.Move[] bestMovesOfNewMoves = SearchMoves(newMoves.ToArray(), true, testBoard);

                int bestMovePieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, bestMovesOfNewMoves[0].startPos, testBoard);

                ChessEngine.Mover.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(bestMovePieceType), bestMovePieceType, bestMovesOfNewMoves[0].startPos, bestMovesOfNewMoves[0].endPos, ref testBoard);

                float evaluation = ChessEngine.evaluation.Evaluate(testBoard, true);

                Debug.Log(evaluation + " : Evaluation");

                bestWhiteMoveEval[i] = evaluation;
            }

            int bestMoveIndex = 0;
            float worstWhiteEval = float.MaxValue;

            for (int i = 0; i < bestMoves.Length; i++)
            {
                if (bestWhiteMoveEval[i] < worstWhiteEval)
                {
                    bestMoveIndex = i;
                    worstWhiteEval = bestWhiteMoveEval[i];
                }
            }


            return bestMoves[bestMoveIndex];
        }

        public MovePieces.Move[] SearchMoves(MovePieces.Move[][] moves, bool WhiteToMove)
        {
            List<MovePieces.Move> bestMove = new();
            float bestMoveEval = float.NegativeInfinity;

            for (int y = 0; y < moves.Length; y++)
            {
                for (int x = 0; x < moves[y].Length; x++)
                { 
                    ChessBoard newBoard = (ChessBoard)ChessEngine.board.Clone();
                    MovePieces mover = new();

                    int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, moves[y][x].startPos, newBoard);

                    mover.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref newBoard), pieceType, moves[y][x].startPos, moves[y][x].endPos, ref newBoard);

                    float moveEval;

                    if (!ChessEngine.WhiteToMove) { moveEval = -ChessEngine.evaluation.Evaluate(newBoard, WhiteToMove); }
                    else
                    {
                        moveEval = ChessEngine.evaluation.Evaluate(newBoard, ChessEngine.WhiteToMove);
                    }

                    if (moveEval > bestMoveEval)
                    {
                        bestMove.Clear();
                        bestMoveEval = moveEval;
                        bestMove.Add(moves[y][x]);
                    }
                    if (moveEval == bestMoveEval)
                    {
                        bestMoveEval = moveEval;
                        bestMove.Add(moves[y][x]);
                    }
                }
            }
            return bestMove.ToArray();
        }
        public MovePieces.Move[] SearchMoves(MovePieces.Move[][] moves, bool WhiteToMove, ChessBoard board)
        {
            List<MovePieces.Move> bestMove = new();
            float bestMoveEval = float.NegativeInfinity;

            for (int y = 0; y < moves.Length; y++)
            {
                for (int x = 0; x < moves[y].Length; x++)
                {
                    ChessBoard newBoard = (ChessBoard)board.Clone();
                    MovePieces mover = new();

                    int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, moves[y][x].startPos, newBoard);

                    mover.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref newBoard), pieceType, moves[y][x].startPos, moves[y][x].endPos, ref newBoard);

                    float moveEval;

                    if (!ChessEngine.WhiteToMove) { moveEval = -ChessEngine.evaluation.Evaluate(newBoard, WhiteToMove); }
                    else
                    {
                        moveEval = ChessEngine.evaluation.Evaluate(newBoard, ChessEngine.WhiteToMove);
                    }

                    if (moveEval > bestMoveEval)
                    {
                        bestMove.Clear();
                        bestMoveEval = moveEval;
                        bestMove.Add(moves[y][x]);
                    }
                    if (moveEval == bestMoveEval)
                    {
                        bestMoveEval = moveEval;
                        bestMove.Add(moves[y][x]);
                    }
                }
            }
            return bestMove.ToArray();
        }
    }
}