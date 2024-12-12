using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChessEngine
{
    public class Search
    {
        public MovePieces.Move SearchAllMoves(int depth, bool WhiteToMove)
        {
            MovePieces mover = new();
            Evaluation evaluator = new();

            bool CurrentWhiteToMove = WhiteToMove;

            MovePieces.Move[][] PossibleMoves = mover.GetMovesForBlackOrWhite(CurrentWhiteToMove);
            MovePieces.Move[] currentBestMoves = SearchMoves(PossibleMoves, CurrentWhiteToMove);

            List<MovePieces.Move> worstMoveList = new();
            MovePieces.Move worstMove = new();
            float worstEval = int.MaxValue;

            for (int currentDepth = 0; currentDepth < 1; currentDepth++)
            {
                for (int i = 0; i < currentBestMoves.Length; i++)
                {
                    ChessBoard testBoard = (ChessBoard)ChessEngine.board.Clone();
                    MovePieces.Move currentMove = currentBestMoves[i];


                    int pieceIndex = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, currentMove.startPos, testBoard);

                    mover.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceIndex, ref testBoard), pieceIndex, currentMove.startPos, currentMove.endPos, ref testBoard);

                    CurrentWhiteToMove = !CurrentWhiteToMove;

                    MovePieces.Move[][] SearchPossibleMoves = mover.GetMovesForBlackOrWhite(CurrentWhiteToMove, testBoard);
                    MovePieces.Move[] SearchBestMoves = SearchMoves(SearchPossibleMoves, CurrentWhiteToMove, testBoard);

                    int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, SearchBestMoves[0].startPos, testBoard);

                    mover.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref testBoard), pieceType, SearchBestMoves[0].startPos, SearchBestMoves[0].endPos, ref testBoard);
                    float evaluationOfMove = evaluator.Evaluate(testBoard, CurrentWhiteToMove);

                    if (evaluationOfMove < worstEval)
                    {
                        worstEval = evaluationOfMove;
                        worstMoveList.Clear();
                        worstMoveList.Add(currentBestMoves[i]);
                    }

                    if (evaluationOfMove == worstEval)
                    {
                        worstMoveList.Add(currentBestMoves[i]);
                    }
                }
            }

            System.Random random = new System.Random();
            worstMove = worstMoveList.ToArray()[random.Next(worstMoveList.Count)];

            return worstMove;
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


                    newBoard.PrintBoard();

                    int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, moves[y][x].startPos, newBoard);

                    Debug.Log(moves[y][x].startPos + " , " + moves[y][x].endPos);

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