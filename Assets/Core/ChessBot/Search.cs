using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;

namespace ChessEngine
{
    public class Search
    {
        string output = "";

        /*public MovePieces.Move RecursiveSearchMoves(int depth, bool WhiteToMove, ChessBoard board)
        {
            MovePieces mover = new();
            Evaluation evaluation = new Evaluation();

            ChessBoard copyBoard = (ChessBoard)board.Clone();

            bool IsWhiteToMove = WhiteToMove;

            MovePieces.Move[] baseMoves = mover.GetMovesForBlackOrWhite(IsWhiteToMove, board);
            List<MovePieces.Move> worstMovesForBaseMoves = new();
            float worstEval = float.PositiveInfinity;

            for (int i = 0; i < baseMoves.Length; i++)
            {
                float eval = GetEvalInDepthRecursive(0, depth, baseMoves[i], copyBoard, IsWhiteToMove);
                if (eval<worstEval)
                {
                    worstEval=eval;
                    worstMovesForBaseMoves.Clear();
                    worstMovesForBaseMoves.Add(baseMoves[i]);
                }
                else if (eval==worstEval)
                {
                    worstMovesForBaseMoves.Add(baseMoves[i]);
                }
            }

            Debug.Log(worstMovesForBaseMoves.Count);

            string path = Application.persistentDataPath + "/savefile.txt";
            using (StreamWriter writer = new StreamWriter(path, false)) // `true` appends instead of overwriting
            {
                writer.WriteLine(output);
            }

            

            System.Random rng = new System.Random();
            return worstMovesForBaseMoves[rng.Next(0,worstMovesForBaseMoves.Count)];
        }*/

        /*public float GetEvalInDepthRecursive(int currentDepth, int maxDepth, MovePieces.Move move, ChessBoard board, bool WhiteToMove)
        {
            currentDepth++;
  
            MovePieces mover = new();
            Evaluation evaluation = new Evaluation();

            ChessBoard copyBoard = (ChessBoard)board.Clone();

            int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, move.startPos, copyBoard);
            mover.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref copyBoard), pieceType, move.startPos, move.endPos, ref copyBoard);


            if (currentDepth==maxDepth)
            {
                bool IsWhiteToMove = !WhiteToMove;

                float eval = evaluation.Evaluate(copyBoard, IsWhiteToMove);
                Debug.Log(eval);
                return eval;
            }
            else
            {
                bool IsWhiteToMove = !WhiteToMove;
                float worstEval = IsWhiteToMove ? float.PositiveInfinity : float.NegativeInfinity;

                MovePieces.Move[] baseMoves = mover.GetMovesForBlackOrWhite(IsWhiteToMove, copyBoard);
                for (int i = 0; i < baseMoves.Length; i++)
                {
                    Debug.Log(baseMoves.Length + " LENGTH OF MOVES LIST");
                    Debug.Log(currentDepth + " DEPTH CURRENTLY");
                    float eval = GetEvalInDepthRecursive(currentDepth, maxDepth, baseMoves[i], copyBoard, IsWhiteToMove);
                    
                    if (!IsWhiteToMove) {
                        if (eval>worstEval)
                        {
                            worstEval=eval;
                        }
                    } else {
                        if (eval<worstEval) {
                            worstEval=eval;
                        }
                    }
                }

                return worstEval;
            } 
        }*/

        public MovePieces.Move IterativeSearchAllMoves(int depth, bool isWhiteToMove, ChessBoard board)
        {
            MovePieces mover = new();
            Evaluation evaluation = new();
            MovePieces.Move bestMove = new();
            float bestEval = float.PositiveInfinity;

            List<MovePieces.Move> rootMoves = mover.GetMovesForBlackOrWhite(isWhiteToMove, board).ToList();

            foreach (var move in rootMoves)
            {
                ChessBoard newBoard = (ChessBoard)board.Clone();
                int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, move.startPos, newBoard);
                mover.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref newBoard), pieceType, move.startPos, move.endPos, ref newBoard);

                float eval = Minimax(newBoard, depth - 1, !isWhiteToMove, evaluation, mover, float.NegativeInfinity, float.PositiveInfinity);

                if (eval < bestEval)
                {
                    bestEval = eval;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        private float Minimax(ChessBoard board, int depth, bool isWhiteToMove, Evaluation evaluation, MovePieces mover, float alpha, float beta)
        {
            if (depth == 0)
            {
                return evaluation.Evaluate(board, isWhiteToMove);
            }

            List<MovePieces.Move> moves = mover.GetMovesForBlackOrWhite(isWhiteToMove, board).ToList();
            float bestEval = isWhiteToMove ? float.NegativeInfinity : float.PositiveInfinity;

            foreach (var move in moves)
            {
                ChessBoard newBoard = (ChessBoard)board.Clone();
                int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, move.startPos, newBoard);
                mover.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref newBoard), pieceType, move.startPos, move.endPos, ref newBoard);

                float eval = Minimax(newBoard, depth - 1, !isWhiteToMove, evaluation, mover, alpha, beta);

                if (isWhiteToMove)
                {
                    bestEval = Math.Max(bestEval, eval);
                    alpha = Math.Max(alpha, eval);
                }
                else
                {
                    bestEval = Math.Min(bestEval, eval);
                    beta = Math.Min(beta, eval);
                }

                // Alpha-beta pruning
                if (beta <= alpha)
                    break;
            }

            return bestEval;
        }





        public MovePieces.Move SearchAllMoves(int depth, bool WhiteToMove)
        {
            MovePieces mover = new();
            Evaluation evaluation = new Evaluation();

            // copy of current chess board
            ChessBoard copyBoard = (ChessBoard)ChessEngine.board.Clone();

            bool IsWhiteToMove = WhiteToMove;

            MovePieces.Move[] originalMoves = null;

            List<float> evaluationBoard = new();

            for (int depthCounter = 0; depthCounter < depth; depthCounter++)
            {
                if (originalMoves == null)
                {
                    MovePieces.Move[] LegalMovesFromCurrentPositions = mover.GetMovesForBlackOrWhite(IsWhiteToMove, copyBoard);



                    originalMoves = LegalMovesFromCurrentPositions;
                    IsWhiteToMove = !IsWhiteToMove;
                }


            }

            // Check evaluation by going through whites moves for a depth of 2
            ChessBoard copyBoardOnStart2 = (ChessBoard)copyBoard.Clone();
            for (int j = 0; j < originalMoves.Length; j++)
            {
                List<float> subEvaluationBoard = new List<float>();

                int pieceType2 = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, originalMoves[j].startPos, copyBoard);

                mover.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType2, ref copyBoard), pieceType2, originalMoves[j].startPos, originalMoves[j].endPos, ref copyBoard);


                MovePieces.Move[] LegalMovesFromCurrentPositions = mover.GetMovesForBlackOrWhite(IsWhiteToMove, copyBoard);

                MovePieces.Move[] bestMovesFromCurrentPosition = LegalMovesFromCurrentPositions;

                for (int i = 0; i < bestMovesFromCurrentPosition.Length; i++)
                {

                    int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, bestMovesFromCurrentPosition[i].startPos, copyBoard);

                    mover.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref copyBoard), pieceType, bestMovesFromCurrentPosition[i].startPos, bestMovesFromCurrentPosition[i].endPos, ref copyBoard);


                    subEvaluationBoard.Add(evaluation.Evaluate(copyBoard, WhiteToMove));
                    copyBoard = (ChessBoard)copyBoardOnStart2.Clone();
                }



                float subBestMoveEvaluation = float.PositiveInfinity;

                for (int c = 0; c < bestMovesFromCurrentPosition.Length; c++)
                {

                    if (subEvaluationBoard[c] > subBestMoveEvaluation)
                    {
                        subBestMoveEvaluation = subEvaluationBoard[c];
                    }
                }

                evaluationBoard.Add(subBestMoveEvaluation);
            }



            float bestMoveEvaluation = float.PositiveInfinity;

            List<MovePieces.Move> bestMovesList = new();

            for (int i = 0; i < originalMoves.Length; i++)
            {
                if (evaluationBoard[i] > bestMoveEvaluation)
                {
                    bestMovesList.Clear();
                    bestMovesList.Add(originalMoves[i]);
                    bestMoveEvaluation = evaluationBoard[i];

                    Debug.Log(bestMoveEvaluation);
                }
                if (evaluationBoard[i] == bestMoveEvaluation)
                {
                    bestMovesList.Add(originalMoves[i]);
                }
            }



            if (bestMovesList.Count > 1)
            {
                System.Random rng = new System.Random();

                return bestMovesList[rng.Next(0, bestMovesList.Count)];
            }

            return bestMovesList[0];
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

                    if (!ChessEngine.board.WhiteToMove) { moveEval = -ChessEngine.evaluation.Evaluate(newBoard, WhiteToMove); }
                    else
                    {
                        moveEval = ChessEngine.evaluation.Evaluate(newBoard, ChessEngine.board.WhiteToMove);
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

                    if (!ChessEngine.board.WhiteToMove) { moveEval = -ChessEngine.evaluation.Evaluate(newBoard, WhiteToMove); }
                    else
                    {
                        moveEval = ChessEngine.evaluation.Evaluate(newBoard, ChessEngine.board.WhiteToMove);
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

    public class MoveAndEval 
    {
        public MovePieces.Move move;
        public float eval;

        public MoveAndEval(MovePieces.Move move, float eval)
        {
            this.move = move;
            this.eval = eval;
        }
    }
}