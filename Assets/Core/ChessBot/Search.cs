using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChessEngine
{
    public class Search
    {
        // MIGHT HAVE SOME PROBLEMS WITH IsWhiteToMove variable
        public MovePieces.Move RefactoredSearchAllMoves(int depth, bool WhiteToMove, ChessBoard board)
        {
            MovePieces move = new();
            Evaluation evaluation = new();

            ChessBoard copyBoard = (ChessBoard)board.Clone();
            bool IsWhiteToMove = WhiteToMove;

            // Which moves gives best eval for black   THE BETTER IF ITS NEGATIVE
            float bestEval = float.PositiveInfinity;
            List<int> bestEvalIndex = new();

            MovePieces.Move[][] moves = new MovePieces.Move[depth][];

            for (int depthCounter = 0; depthCounter < depth; depthCounter++)
            {
                
                if (depthCounter == 0)
                {
                    Debug.Log(WhiteToMove);
                    MovePieces.Move[] LegalMovesFromCurrentPosition = move.GetMovesForBlackOrWhite(IsWhiteToMove, copyBoard);

                    moves[depthCounter] = LegalMovesFromCurrentPosition;

                    IsWhiteToMove = !IsWhiteToMove;
                }
                else
                {
                    Debug.Log(WhiteToMove);
                    // Create copy of board so that multiple of the moves dont happen on the same boards attempting to move empty squares after its already moved
                    ChessBoard boardReset = (ChessBoard)copyBoard.Clone();

                    for (int i= 0; i <  moves[depthCounter-1].Length; i++)
                    {

                        int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, moves[depthCounter-1][i].startPos, copyBoard);

                        move.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref copyBoard), pieceType, moves[depthCounter-1][i].startPos, moves[depthCounter-1][i].endPos, ref copyBoard);

                        MovePieces.Move[] LegalMovesFromCurrentPosition = move.GetMovesForBlackOrWhite(IsWhiteToMove, copyBoard);

                        moves[depthCounter] = LegalMovesFromCurrentPosition;

                        copyBoard = (ChessBoard)boardReset.Clone();
                        
                    }

                    IsWhiteToMove = !IsWhiteToMove;
                }


            }

            

            ChessBoard boardReset1 = (ChessBoard)copyBoard.Clone();

            // Evaluate the positions after its found all the moves
            for (int i= 0; i < moves[depth-1].Length; i++)
            {
                int pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, moves[depth - 1][i].startPos, copyBoard);

                move.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref copyBoard), pieceType, moves[depth - 1][i].startPos, moves[depth - 1][i].endPos, ref copyBoard);

                float eval = evaluation.Evaluate(copyBoard, IsWhiteToMove);


                if (eval < bestEval)
                {
                    bestEval = eval;
                    bestEvalIndex.Clear();
                    bestEvalIndex.Add(i);
                    Debug.Log(eval + ": " + moves[depth - 1][i].startPos + " , " + moves[depth - 1][i].endPos);
                }
                if (eval==bestEval)
                {
                    bestEvalIndex.Add(i);
                    Debug.Log(eval + ": " + moves[depth - 1][i].startPos + " , " + moves[depth - 1][i].endPos);
                }


                copyBoard = (ChessBoard)boardReset1.Clone();
            }


            if (bestEvalIndex.Count == 1)
            {
                return moves[depth-1][bestEvalIndex[0]];
            }

            System.Random rng = new System.Random();

            return moves[depth-1][bestEvalIndex[rng.Next(0, bestEvalIndex.Count)]];
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

            for (int i = 0;i < originalMoves.Length; i++)
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