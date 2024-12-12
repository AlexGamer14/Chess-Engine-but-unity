using UnityEngine;

namespace ChessEngine
{
    public class Evaluation
    {

        const float pawnValue = 100;
        const float rookValue = 500;
        const float knightsValue = 300;
        const float bishopsValue = 320;
        const float queenValue = 900;
        const float moveFirstValue = 0;

        bool whiteKingAlive = false;
        bool blackKingAlive = false;

        public float Evaluate(ChessBoard board, bool whiteToMove)
        {
            float evaluation = 0;


            float pieceBonus = PieceBonus(board);
            float defenceBonus = DefenceBonus();

            evaluation += pieceBonus;


            if (whiteToMove == true)
            {
                evaluation += moveFirstValue;
            }
            else
            {
                evaluation -= moveFirstValue;
            }



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

            for (int i = 0; i < 64; i++)
            {
                if (HelperFunctions.GetByte(i, board.WhitePawns) == 1)
                {
                    evaluation += pawnValue;
                }
                if (HelperFunctions.GetByte(i, board.WhiteKing) == 1)
                {
                    whiteKingAlive = true;
                }
                if (HelperFunctions.GetByte(i, board.WhiteKnights) == 1)
                {
                    evaluation += knightsValue;
                }
                if (HelperFunctions.GetByte(i, board.WhiteBishops) == 1)
                {
                    evaluation += bishopsValue;
                }
                if (HelperFunctions.GetByte(i, board.WhiteRooks) == 1)
                {
                    evaluation += rookValue;
                }
                if (HelperFunctions.GetByte(i, board.WhiteQueens) == 1)
                {
                    evaluation += queenValue;
                }

                if (HelperFunctions.GetByte(i, board.BlackPawns) == 1)
                {
                    evaluation -= pawnValue;
                }
                if (HelperFunctions.GetByte(i, board.BlackKing) == 1)
                {
                    blackKingAlive = true;
                }
                if (HelperFunctions.GetByte(i, board.BlackKnights) == 1)
                {
                    evaluation -= knightsValue;
                }
                if (HelperFunctions.GetByte(i, board.BlackBishops) == 1)
                {
                    evaluation -= bishopsValue;
                }
                if (HelperFunctions.GetByte(i, board.BlackRooks) == 1)
                {
                    evaluation -= rookValue;
                }
                if (HelperFunctions.GetByte(i, board.BlackQueens) == 1)
                {
                    evaluation -= queenValue;
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
            return evaluation;
        }

        private float DefenceBonus()
        {
            float evaluation = 0;

            for (int i = 0; i < 64; i++)
            {

            }

            return evaluation;
        }
    }
}