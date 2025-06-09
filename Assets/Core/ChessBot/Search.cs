using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChessEngine
{
    public static class Search
    {
        static TranspositionTable transpositionTable = new(21); // Creates a transposition table of size 2 ^ 20 * 32Bytes  entries rounding to around 32mb;

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

        public static List<OrgMove> baseMovesCheck = new List<OrgMove>();

        public static int Perft(int depth, ChessBoard board, bool isWhiteToMove)
        {
            if (depth == 0)
                return 1; // Count 1 position when we reach the target depth

            int nodes = 0;
             // Preallocate a list to store moves and their counts

            MovePieces.Move[] moves = MovePieces.GetMovesForBlackOrWhite(isWhiteToMove, board);
            foreach (var move in moves)
            {
                ChessBoard newBoard = (ChessBoard)board.Clone();
                int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(move.startPos, newBoard);
                MovePieces.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref newBoard), pieceType, move, ref newBoard);

                int amount = Perft(depth - 1, newBoard, !isWhiteToMove);

                if (depth == ChessEngine.depth)
                {
                    baseMovesCheck.Add(new(move,amount));
                }

                nodes += amount;
            }

            if (depth==ChessEngine.depth)
            {
                HelperFunctions.PrintList(baseMovesCheck);
            }
            return nodes;
        }



        public static MovePieces.Move IterativeSearchAllMoves(int depth, bool isWhiteToMove, ChessBoard board)
        {
            Evaluation evaluation = new();
            MovePieces.Move bestMove = default;
            float bestEval = isWhiteToMove ? float.NegativeInfinity : float.PositiveInfinity;

            MovePieces.Move[] rootMoves = MovePieces.GetMovesForBlackOrWhite(isWhiteToMove, board);

            if (isWhiteToMove)
            {
                Debug.Log(depth);
            }


            foreach (var move in rootMoves)
            {
                ChessBoard newBoard = (ChessBoard)board.Clone();
                int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(move.startPos, newBoard);
                MovePieces.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref newBoard), pieceType, move, ref newBoard);

                float eval = Minimax(newBoard, depth - 1, 0, !isWhiteToMove, evaluation, float.NegativeInfinity, float.PositiveInfinity);

                if (isWhiteToMove)
                {
                    if (eval > bestEval)
                    {
                        bestEval = eval;
                        bestMove = move;
                    }
                }
                else
                {
                    if (eval < bestEval)
                    {
                        bestEval = eval;
                        bestMove = move;
                    }
                }

            }

            Debug.Log("Best eval: " + bestEval);

            return bestMove;
        }

        private static float Minimax(ChessBoard board, int depth, int amountExtended, bool isWhiteToMove, Evaluation evaluation, float alpha, float beta)
        {
            if (depth == 0)
            {
                return evaluation.Evaluate(board, isWhiteToMove);
            }

            float alphaOriginal = alpha;
            ulong zobristKey = board.ComputeZobristHash();

            if (transpositionTable.TryGet(zobristKey, out var ttEntry) && ttEntry.Depth >= depth)
            {
                switch (ttEntry.Type)
                {
                    case TranspositionTable.NodeType.Exact:
                        return ttEntry.Evaluation;
                    case TranspositionTable.NodeType.LowerBound:
                        alpha = Math.Max(alpha, ttEntry.Evaluation);
                        break;
                    case TranspositionTable.NodeType.UpperBound:
                        beta = Math.Min(beta, ttEntry.Evaluation);
                        break;
                }

                if (alpha >= beta)
                    return ttEntry.Evaluation;
            }

            MovePieces.Move[] moves = MovePieces.GetMovesForBlackOrWhite(isWhiteToMove, board);
            if (moves.Length == 0)
            {
                return evaluation.Evaluate(board, isWhiteToMove);
            }


            float bestEval = isWhiteToMove ? float.NegativeInfinity : float.PositiveInfinity;
            MovePieces.Move bestMove = default;


            foreach (var move in moves)
            {
                ChessBoard newBoard = (ChessBoard)board.Clone();
                int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(move.startPos, newBoard);
                MovePieces.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref newBoard), pieceType, move, ref newBoard);

                float eval;

                eval = Minimax(newBoard, depth - 1, amountExtended, !isWhiteToMove, evaluation, alpha, beta);

                if (isWhiteToMove)
                {
                    if (eval > bestEval)
                    {
                        bestEval = eval;
                        bestMove = move;
                    }
                    alpha = Math.Max(alpha, eval);
                }
                else
                {
                    if (eval < bestEval)
                    {
                        bestEval = eval;
                        bestMove = move;
                    }
                    beta = Math.Min(beta, eval);
                }

                // Alpha-beta pruning
                if (beta <= alpha)
                    break;
            }

            TranspositionTable.NodeType type;
            if (bestEval <= alphaOriginal)
                type = TranspositionTable.NodeType.UpperBound;
            else if (bestEval >= beta)
                type = TranspositionTable.NodeType.LowerBound;
            else
                type = TranspositionTable.NodeType.Exact;

            transpositionTable.Store(zobristKey, bestEval, depth, type, bestMove);

            return bestEval;
        }

        public struct OrgMove
        {
            public string PGN;
            public long amount;

            public OrgMove(MovePieces.Move move, long amount)
            {
                PGN = PGNConverter.MoveToPGN(move);
                this.amount = amount;
            }

            public override string ToString()
            {
                return $"{PGN} - {amount}\n";
            }
        }
    }
}