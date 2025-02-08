using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using UnityEngine;

namespace ChessEngine
{
    public class Search
    {
        // MIGHT HAVE SOME PROBLEMS WITH IsWhiteToMove variable
        public MovePieces.Move RefactoredSearchAllMoves(int depth, bool WhiteToMove,ChessBoard board)
        {
            MovePieces mover = new();
            Evaluation evaluation = new Evaluation();

            ChessBoard copyBoard = (ChessBoard)board.Clone();

            bool IsWhiteToMove = WhiteToMove;

            MovePieces.Move[] movesToCheck = new MovePieces.Move[depth];

            List<int> depthList = new List<int>();
            for (int i = 0; i<depth;i++)
            {
                depthList.Add(0);
                
            }

            float bestEval = float.PositiveInfinity;
            List<MovePieces.Move> bestMove = new();

            ChessBoard[] resetLayers = new ChessBoard[depth];
            resetLayers[0] = (ChessBoard)copyBoard.Clone();

            Dictionary<MovePieces.Move, moveAndEval> evalList = new();

            string path = Application.persistentDataPath + "/savefile.txt";

            string output = "";

            Debug.Log("File updated at: " + path);

            bool Calculating = true;
            int calculatedDepth = 0;

            int pieceType = 0;
            while (Calculating)
            {
                switch (calculatedDepth)
                {
                    case 0:
                        
                        copyBoard = (ChessBoard)resetLayers[0].Clone();
                        IsWhiteToMove = WhiteToMove;
                        try
                        {
                            movesToCheck[0]=mover.GetMovesForBlackOrWhite(IsWhiteToMove, copyBoard)[depthList[0]];
                            output+="CHECKING NEW BASE MOVE: " + movesToCheck[0].startPos + " : " + movesToCheck[0].endPos + "\n";
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Calculating = false;
                            break;
                        }
                        depthList[0]++;
                        calculatedDepth++;
                        break;
                    case 1:
                        copyBoard = (ChessBoard)resetLayers[0].Clone();
                        IsWhiteToMove = !WhiteToMove;
                        pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, movesToCheck[0].startPos, copyBoard);

                        mover.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref copyBoard), pieceType, movesToCheck[0].startPos, movesToCheck[0].endPos, ref copyBoard);
                        try
                        {
                            movesToCheck[1]=(mover.GetMovesForBlackOrWhite(IsWhiteToMove,copyBoard)[depthList[1]]);
                            output+="CHECKING NEW SUB MOVE: " + movesToCheck[1].startPos + " : " + movesToCheck[1].endPos + "\n";
                        }
                        catch (IndexOutOfRangeException)
                        {
                            calculatedDepth--;
                            depthList[1]=0;
                            break;
                        }
                        resetLayers[1] = (ChessBoard)copyBoard.Clone();
                        depthList[1]++;
                        calculatedDepth++;
                        break;
                    case 2:
                        copyBoard = (ChessBoard)resetLayers[1].Clone();
                        IsWhiteToMove=WhiteToMove;
                        pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, movesToCheck[1].startPos, copyBoard);

                        

                        mover.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref copyBoard), pieceType, movesToCheck[1].startPos, movesToCheck[1].endPos, ref copyBoard);
                        try
                        {
                            movesToCheck[2]=(mover.GetMovesForBlackOrWhite(IsWhiteToMove,copyBoard)[depthList[2]]);
                            
                            output+="CHECKING NEW SUB SUB MOVE: " + movesToCheck[2].startPos + " : " + movesToCheck[2].endPos+"\n";

                        }
                        catch (IndexOutOfRangeException)
                        {
                            calculatedDepth--;
                            output += depthList[2] + " Amount of moves" + "\n";
                            depthList[2] = 0;
                            break;
                        }

                        depthList[2]++;
                        resetLayers[2]= (ChessBoard)copyBoard.Clone();

                        if (depth > 3)
                        {
                            calculatedDepth++;
                        }

                        else
                        {

                            pieceType = HelperFunctions.CheckIfPieceOnEveryBoard(int.MaxValue, movesToCheck[2].startPos, copyBoard);
                            mover.SearchMovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType, ref copyBoard), pieceType, movesToCheck[2].startPos, movesToCheck[2].endPos, ref copyBoard);

                            float eval = evaluation.Evaluate(copyBoard, IsWhiteToMove);
                            output += eval + "\n";

                            // GETS THE WORST MOVE IN THIS POSITION TO TRY TO SHOW THE BEST OVERALL MOVE
                            if (evalList.ContainsKey(movesToCheck[0]))
                            {
                                if (eval > evalList[movesToCheck[0]].eval)
                                {
                                    MovePieces.Move key = movesToCheck[0];

                                    evalList[key].move = movesToCheck[0];
                                    evalList[key].eval = eval;


                                }
                            }
                            else
                            {
                                MovePieces.Move key = movesToCheck[0];
                                evalList.Add(key, new moveAndEval(movesToCheck[0], eval));
                            }
                        }

                        break;
                }
            }

            foreach (moveAndEval value in evalList.Values)
            {
                if (value.eval < bestEval)
                {
                    bestEval = value.eval;
                    bestMove.Clear();
                    bestMove.Add(value.move);

                    output += "New best eval with base move: " + value.move.startPos + " , " + value.move.endPos +" and eval of: " + value.eval + "\n";
                }

                if (value.eval==bestEval)
                {
                    bestMove.Add(value.move);
                    output += "New equal eval with base move: " + value.move.startPos + " , " + value.move.endPos + " and eval of" + value.eval + "\n";
                }
            }

            using (StreamWriter writer = new StreamWriter(path, false)) // `true` appends instead of overwriting
            {
                writer.WriteLine(output);
            }

            System.Random rng = new();


            return bestMove[rng.Next(0,bestMove.Count)];
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

    public class moveAndEval 
    {
        public MovePieces.Move move;
        public float eval;

        public moveAndEval(MovePieces.Move move, float eval)
        {
            this.move = move;
            this.eval = eval;
        }
    }
}