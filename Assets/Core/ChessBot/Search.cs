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
            //Debug.Log("Is this a memory leak? (in iterativesearchallmoves)");
            MovePieces mover = new();
            Evaluation evaluation = new();
            MovePieces.Move bestMove = new();
            float bestEval = float.PositiveInfinity;

            MovePieces.Move[] rootMoves = mover.GetMovesForBlackOrWhite(isWhiteToMove, board);

            foreach (var move in rootMoves)
            {
                ChessBoard newBoard = (ChessBoard)board.Clone();
                int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, move.startPos, newBoard);
                mover.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref newBoard), pieceType, move, ref newBoard);

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

            MovePieces.Move[] moves = mover.GetMovesForBlackOrWhite(isWhiteToMove, board);
            float bestEval = isWhiteToMove ? float.NegativeInfinity : float.PositiveInfinity;

            foreach (var move in moves)
            {
                ChessBoard newBoard = (ChessBoard)board.Clone();
                int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, move.startPos, newBoard);
                mover.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref newBoard), pieceType, move, ref newBoard);

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