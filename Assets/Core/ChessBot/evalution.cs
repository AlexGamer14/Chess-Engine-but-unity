using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ChessEngine
{
    public class Evaluation
    {
        const float pawnValue = 100;
        const float rookValue = 500;
        const float knightsValue = 300;
        const float bishopsValue = 350;
        const float queenValue = 900;

        bool whiteKingAlive = false;
        bool blackKingAlive = false;

        int[] earlyBlackKingBonus = new int[64] { -20, -20, -20, -20, -20, -20, -20, -20,
-20, -20, -20, -20, -20, -20, -20, -20,
-20, -20, -20, -20, -20, -20, -20, -20,
-20, -20, -20, -20, -20, -20, -20, -20,
-20, -20, -20, -20, -20, -20, -20, -20,
-20, -20, -20, -20, -20, -20, -20, -20,
-20, -20, -20, -20, -20, -20, -20, -20,
10, 5, 5, 5, 15, 5, 5, 10, };

        int[] earlyWhiteKingBonus = new int[64] { 10, 5, 5, 15, 5, 5, 5, 10,
-20, -20, -20, -20, -20, -20, -20, -20,
-20, -20, -20, -20, -20, -20, -20, -20,
-20, -20, -20, -20, -20, -20, -20, -20,
-20, -20, -20, -20, -20, -20, -20, -20,
-20, -20, -20, -20, -20, -20, -20, -20,
-20, -20, -20, -20, -20, -20, -20, -20,
-20, -20, -20, -20, -20, -20, -20, -20, };

        int[] earlyWhitePawnBonus = new int[64] { 0, 0, 0, 0, 0, 0,  0, 0,
                                        10, 15, 10, 5,  5,  10, 15, 10,
                                        5,  0,  5, 15, 15, 5, 0,  5,
                                        0,  0,  10, 25, 25, 10, 0,  0,
                                        0,  0,  0, 10,  10,  0, 0,  0,
                                        0,  0,  0, 0,  0,  0, 0,  0,
                                        0,  0,  0, 0,  0,  0, 0,  0,
                                        0,  0,  0, 0,  0,  0, 0,  0,
                                       };

        int[] earlyBlackKnightBonus = new int[64] { 0, 0, 0, 0, 0, 0, 0, 0,
                                                    0, 0, 0, 0, 0, 0, 0, 0,
                                                    0, 0, 0, 0, 0, 0, 0, 0,
                                                    0, 10, 0, 15, 15, 0, 10, 0,
                                                    5, 0, 0, 0, 0, 0, 0, 5,
                                                    0, 0, 20, 0, 0, 20, 0, 0,
                                                    -5, 0, 0, 10, 10, 0, 0, -5,
                                                    0, 5, 0, 0, 0, 0, 5, 0 };

        int[] earlyWhiteKnightBonus = new int[64] { 0, 5, 0, 0, 0, 0, 5, 0,
                                                    -5, 0, 0, 10, 10, 0, 0, -5,
                                                    0, 0, 20, 0, 0, 20, 0, 0,
                                                    5, 0, 0, 0, 0, 0, 0, 5,
                                                    0, 10, 0, 15, 15, 0, 10, 0,
                                                    0, 0, 0, 0, 0, 0, 0, 0,
                                                    0, 0, 0, 0, 0, 0, 0, 0,
                                                    0, 0, 0, 0, 0, 0, 0, 0 };

        int[] earlyWhiteRookBonus = new int[64] { 20, 5, 5, 5, 5, 5, 5, 20,
0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0, };

        int[] earlyBlackRookBonus = new int[64] { 0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0,
20, 5, 5, 5, 5, 5, 5, 20, };

        int[] earlyBlackPawnBonus = new int[64] {
    0,  0,  0,  0,  0,  0,  0,  0,
    0,  0,  0,  0,  0,  0,  0,  0,
    0,  0,  0,  0,  0,  0,  0,  0,
    0,  0,  0, 10, 10,  0,  0,  0,
    0,  0, 10, 25, 25, 10,  0,  0,
    5,  0,  5, 15, 15,  5,  0,  5,
    10, 15,10,  5,  5,  10, 15, 10,
    0,  0,  0,  0,  0,  0,  0,  0
};

        int[] earlyBishopBonus = new int[64] {- 5, -5, -5, -5, -5, -5, -5, -5,
-5, 0, 0, 0, 0, 0, 0, -5,
-5, 5, 10, 10, 10, 10, 5, -5,
-5, 5, 10, 15, 15, 10, 5, -5,
-5, 5, 10, 15, 15, 10, 5, -5,
-5, 5, 10, 10, 10, 10, 5, -5,
-5, 0, 0, 0, 0, 0, 0, -5,
-5, -5, -5, -5, -5, -5, -5, -5, };

        public float Evaluate(ChessBoard board, bool whiteToMove)
        {
            float evaluation = 0;


            float pieceBonus = PieceBonus(board);

            float checkMateBonus = CheckMateBonus(board, whiteToMove, evaluation);
            float controlSquaresBonus = ControlSquaresBonus(board);
            

            evaluation += pieceBonus;
            evaluation += controlSquaresBonus;
            evaluation+= checkMateBonus;

            // Stalemate calculation uses 96% of the time for every move
            //float staleMateBonus = StaleMateBonus(board, evaluation);
            //evaluation += staleMateBonus;


            return evaluation;
        }

        private float PieceBonus(ChessBoard board)
        {
            float evaluation = 0;

            whiteKingAlive = false;
            blackKingAlive = false;

            int amountOfPieces = 0;

            List<int> whitePawnPositions = new(64);
            List<int> blackPawnPositions = new(64);
            List<int> whiteKnightPositions = new(64);
            List<int> blackKnightPositions = new(64);
            List<int> whiteKingPositions = new(64);
            List<int> blackKingPositions = new(64);
            List<int> whiteRookPositions = new(64);
            List<int> blackRookPositions = new(64);
            List<int> whiteBishopPositions = new(64);
            List<int> blackBishopPositions = new(64);
            List<int> whiteQueenPositions = new(64);
            List<int> blackQueenPositions = new(64);

            for (int i = 0; i < 64; i++)
            {
                if (HelperFunctions.GetBit(i, board.AllPieces) == 0)
                    continue; // Skip empty squares

                if (HelperFunctions.GetBit(i, board.WhitePawns) == 1)
                {
                    evaluation += pawnValue;
                    whitePawnPositions.Add(i);
                    //Debug.Log(i);

                    amountOfPieces++;
                }
                if (HelperFunctions.GetBit(i, board.WhiteKing) == 1)
                {
                    whiteKingPositions.Add(i);
                    whiteKingAlive = true;
                    amountOfPieces++;
                }
                if (HelperFunctions.GetBit(i, board.WhiteKnights) == 1)
                {
                    whiteKnightPositions.Add(i);
                    evaluation += knightsValue;
                    amountOfPieces++;
                }
                if (HelperFunctions.GetBit(i, board.WhiteBishops) == 1)
                {
                    evaluation += bishopsValue;
                    whiteBishopPositions.Add(i);
                    amountOfPieces++;
                }
                if (HelperFunctions.GetBit(i, board.WhiteRooks) == 1)
                {
                    evaluation += rookValue;
                    whiteRookPositions.Add(i);
                    amountOfPieces++;
                }
                if (HelperFunctions.GetBit(i, board.WhiteQueens) == 1)
                {
                    evaluation += queenValue;
                    whiteQueenPositions.Add(i);
                    amountOfPieces++;
                }

                if (HelperFunctions.GetBit(i, board.BlackPawns) == 1)
                {
                    evaluation -= pawnValue;

                    blackPawnPositions.Add(i);

                    amountOfPieces++;
                }
                if (HelperFunctions.GetBit(i, board.BlackKing) == 1)
                {
                    blackKingPositions.Add(i);
                    blackKingAlive = true;
                    amountOfPieces++;
                }
                if (HelperFunctions.GetBit(i, board.BlackKnights) == 1)
                {
                    blackKnightPositions.Add(i);
                    evaluation -= knightsValue;
                    amountOfPieces++;
                }
                if (HelperFunctions.GetBit(i, board.BlackBishops) == 1)
                {
                    evaluation -= bishopsValue;
                    blackBishopPositions.Add(i);
                    amountOfPieces++;
                }
                if (HelperFunctions.GetBit(i, board.BlackRooks) == 1)
                {
                    evaluation -= rookValue;
                    blackRookPositions.Add(i);
                    amountOfPieces++;
                }
                if (HelperFunctions.GetBit(i, board.BlackQueens) == 1)
                {
                    evaluation -= queenValue;
                    blackQueenPositions.Add(i);
                    amountOfPieces++;
                }
            }



            if (board.MoveCount < 20)
            {
                foreach (int pos in whitePawnPositions)
                {
                    evaluation += earlyWhitePawnBonus[pos];
                }
                foreach (int pos in blackPawnPositions)
                {
                    evaluation -= earlyBlackPawnBonus[pos];
                }
                foreach (int pos in blackKnightPositions)
                {
                    evaluation -= earlyBlackKnightBonus[pos];
                }
                foreach (int pos in whiteKnightPositions)
                {
                    evaluation += earlyWhiteKnightBonus[pos];
                }
                foreach (int pos in whiteKingPositions)
                {
                    evaluation += earlyWhiteKingBonus[pos];
                }
                foreach (int pos in blackKingPositions)
                {
                    evaluation -= earlyBlackKingBonus[pos];
                }

                foreach (int pos in whiteRookPositions)
                {
                    evaluation += earlyWhiteRookBonus[pos];
                }
                foreach (int pos in blackRookPositions)
                {
                    evaluation -= earlyBlackRookBonus[pos];
                }

                foreach (int pos in whiteBishopPositions)
                {
                    evaluation += earlyBishopBonus[pos];
                }
                foreach (int pos in blackBishopPositions)
                {
                    evaluation -= earlyBishopBonus[pos];
                }

            }

            // These values are probably to high
            if (!blackKingAlive)
            {
                evaluation += 8000;
            }

            if (!whiteKingAlive)
            {
                evaluation -= 8000;
            }

            return evaluation;
        }

        private static float CheckMateBonus(ChessBoard board, bool white, float current_eval)
        {
            float evaluation = 0;

            bool IsWhiteCheckeMate = board.IsWhiteCheckMate();
            bool IsBlackCheckeMate = board.IsBlackCheckMate();

            if (IsWhiteCheckeMate)
            {
                evaluation -= 10000;
            }
            if (IsBlackCheckeMate)
            {
                evaluation += 10000;
            }

            return evaluation;
        }

        public float StaleMateBonus(ChessBoard board, float current_eval)
        {
            float evaluation = 0;

            bool IsWhiteStaleMate = board.IsWhiteStaleMate();
            bool IsBlackStaleMate = board.IsBlackStaleMate();

            if (IsWhiteStaleMate)
            {
                evaluation -= 1000;
            }
            if (IsBlackStaleMate)
            {
                evaluation += 1000;
            }

            return evaluation;
        }

        private float CheckBonus(ChessBoard board)
        {
            float evaluation = 0;

            if (board.IsWhiteChecked())
            {
                evaluation -= 20;
            }
            if (board.IsBlackChecked())
            {
                evaluation += 20;
            }

            return evaluation;
        }

        private float ControlSquaresBonus(ChessBoard board)
        {
            float evaluation = 0;
            float perSquareBonus = 3;

            for (int i = 0; i < 64; i++)
            {
                // Control Squares Bonus
                if (HelperFunctions.GetBit(i, board.WhiteAttackBoard)==1)
                {
                    evaluation += perSquareBonus;
                }
                if (HelperFunctions.GetBit(i, board.BlackAttackBoard) == 1)
                {
                    evaluation -= perSquareBonus;
                }

                // Defence Bonus
                if (HelperFunctions.GetBit(i, board.BlackPieces) == 1 && HelperFunctions.GetBit(i, board.BlackAttackBoard)==1)
                {
                    evaluation -= 10;
                }
                if (HelperFunctions.GetBit(i, board.WhitePieces) == 1 && HelperFunctions.GetBit(i, board.WhiteAttackBoard)==1)
                {
                    evaluation += 10;
                }

                // Attack Bonus
                if (HelperFunctions.GetBit(i, board.BlackPieces) == 1 && HelperFunctions.GetBit(i, board.WhiteAttackBoard)==1)
                {
                    evaluation += 10;
                }
                if (HelperFunctions.GetBit(i, board.WhitePieces) == 1 && HelperFunctions.GetBit(i, board.BlackAttackBoard)==1)
                {
                    evaluation -= 10;
                }
            }
            return evaluation;
        }

    }
}