using System.Collections.Generic;
using UnityEngine;

namespace ChessEngine
{
    public class Evaluation
    {

        const float pawnValue = 100;
        const float rookValue = 500;
        const float knightsValue = 350;
        const float bishopsValue = 350;
        const float queenValue = 900;
        const float moveFirstValue = 0;

        bool whiteKingAlive = false;
        bool blackKingAlive = false;

        int[] earlyWhitePawnBonus = new int[64] { 0, 0, 0, 0, 0, 0,  0, 0,
                                        10, 15, 10, 5,  5,  10, 15, 10,
                                        5,  0,  5, 15, 15, 5, 0,  5,
                                        0,  0,  10, 25, 25, 10, 0,  0,
                                        0,  0,  0, 0,  0,  0, 0,  0,
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

        /*int[] earlyWhiteKnightBonus = new int[64] { 0, 0, 0, 0, 0, 0, 0, 0,
                                                    0, 0, 0, 0, 0, 0, 0, 0,
                                                    0, 0, 0, 0, 0, 0, 0, 0,
                                                    0, 10, 0, 20, 20, 0, 10, 0,
                                                    5, 0, 0, 0, 0, 0, 0, 5,
                                                    0, 0, 25, 0, 0, 25, 0, 0,
                                                    -5, 0, 0, 15, 15, 0, 0, -5,
                                                    0, 5, 0, 0, 0, 0, 5, 0 };*/

        int[] earlyBlackPawnBonus = new int[64] {
    0,  0,  0,  0,  0,  0,  0,  0,
    0,  0,  0,  0,  0,  0,  0,  0,
    0,  0,  0,  0,  0,  0,  0,  0,
    0,  0,  0,  0,  0,  0,  0,  0,
    0,  0, 10, 25, 25, 10,  0,  0,
    5,  0,  5, 15, 15,  5,  0,  5,
    10, 15,10,  5,  5,  10, 15, 10,
    0,  0,  0,  0,  0,  0,  0,  0
};



        public float Evaluate(ChessBoard board, bool whiteToMove)
        {
            float evaluation = 0;


            float pieceBonus = PieceBonus(board);

            evaluation += pieceBonus;


            if (!whiteKingAlive)
            {
                evaluation -= 200000;
                Debug.Log("Black won");
            }
            if (!blackKingAlive)
            {
                evaluation += 200000;
                Debug.Log("White won");
            }

            return evaluation;
        }

        private float PieceBonus(ChessBoard board)
        {
            float evaluation = 0;

            whiteKingAlive = false;
            blackKingAlive = false;

            int amountOfPieces = 0;

            List<int> whitePawnPositions = new List<int>();
            List<int> blackPawnPositions = new List<int>();

            for (int i = 0; i < 64; i++)
            {
                if (HelperFunctions.GetByte(i, board.WhitePawns) == 1)
                {
                    evaluation += pawnValue;
                    whitePawnPositions.Add(i);
                    Debug.Log(i);

                    amountOfPieces++;
                }
                if (HelperFunctions.GetByte(i, board.WhiteKing) == 1)
                {
                    whiteKingAlive = true;
                    amountOfPieces++;
                }
                if (HelperFunctions.GetByte(i, board.WhiteKnights) == 1)
                {
                    evaluation += knightsValue;
                    amountOfPieces++;
                }
                if (HelperFunctions.GetByte(i, board.WhiteBishops) == 1)
                {
                    evaluation += bishopsValue;
                    amountOfPieces++;
                }
                if (HelperFunctions.GetByte(i, board.WhiteRooks) == 1)
                {
                    evaluation += rookValue;
                    amountOfPieces++;
                }
                if (HelperFunctions.GetByte(i, board.WhiteQueens) == 1)
                {
                    evaluation += queenValue;

                    if (i==3)
                    {
                        evaluation += 40;
                    }
                    amountOfPieces++;
                }

                if (HelperFunctions.GetByte(i, board.BlackPawns) == 1)
                {
                    evaluation -= pawnValue;

                    blackPawnPositions.Add(i);

                    amountOfPieces++;
                }
                if (HelperFunctions.GetByte(i, board.BlackKing) == 1)
                {
                    blackKingAlive = true;
                    amountOfPieces++;
                }
                if (HelperFunctions.GetByte(i, board.BlackKnights) == 1)
                {
                    evaluation -= knightsValue;
                    amountOfPieces++;
                }
                if (HelperFunctions.GetByte(i, board.BlackBishops) == 1)
                {
                    evaluation -= bishopsValue;
                    amountOfPieces++;
                }
                if (HelperFunctions.GetByte(i, board.BlackRooks) == 1)
                {
                    evaluation -= rookValue;
                    amountOfPieces++;
                }
                if (HelperFunctions.GetByte(i, board.BlackQueens) == 1)
                {
                    evaluation -= queenValue;
                    amountOfPieces++;

                    if (i==59)
                    {
                        evaluation -= 40;
                    }
                }
            }

            if (!blackKingAlive)
            {
                evaluation += 1000000;
                Debug.Log("White won!");
            }
            if (!whiteKingAlive)
            {
                evaluation -= 1000000;
                Debug.Log("Black won!");
            }


            if (amountOfPieces>11)
            {
                foreach (int pos in whitePawnPositions)
                {
                    evaluation += earlyWhitePawnBonus[pos];
                }
                foreach (int pos in blackPawnPositions)
                {
                    evaluation -= earlyBlackPawnBonus[pos];
                }
                foreach (int pos in HelperFunctions.BitboardToList(board.BlackKnights))
                {
                    evaluation -= earlyBlackKnightBonus[pos];
                }

            }
            
            


            return evaluation;
        }

        /*private float DefenceBonus()
        {
            float evaluation = 0;

            for (int i = 0; i < 64; i++)
            {
  
            }

            return evaluation;
        }*/
    }
}