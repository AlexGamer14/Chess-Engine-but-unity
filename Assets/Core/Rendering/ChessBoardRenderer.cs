using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;


namespace ChessEngine {
    public class ChessBoardRenderer
    {
        private readonly int size = 8;
        public GameObject[,] board;
        public GameObject[,] MoveBoard;

        public GameObject[,] attackBoardBoard;


        public Sprite[] spriteSheet;

        

        public void Initialize(Transform parentObj, Sprite[] sprites, GameObject prefab, GameObject movePrefab, GameObject AttackBoardPrefab)
        {
            board = new GameObject[size, size];
            MoveBoard = new GameObject[size, size];
            attackBoardBoard = new GameObject[size, size];
            spriteSheet = sprites;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    board[y,x] = GameObject.Instantiate(prefab);
                    MoveBoard[y,x] = GameObject.Instantiate (movePrefab);

                    attackBoardBoard[y, x] = GameObject.Instantiate(AttackBoardPrefab);

                    board[y,x].transform.SetParent( parentObj);
                    MoveBoard[y, x].transform.SetParent(parentObj);

                    attackBoardBoard[y, x].transform.SetParent(parentObj);

                    board[y,x].GetComponent<RectTransform>().anchoredPosition = new Vector3(-350+(x*100), -350+(y*100), 0);
                    attackBoardBoard[y, x].GetComponent<RectTransform>().anchoredPosition = new Vector3(-350 + (x * 100), -350 + (y * 100), 0);
                    MoveBoard[y,x].GetComponent<RectTransform>().anchoredPosition = new Vector3(-350 + (x * 100), -350 + (y * 100), 0);

                    attackBoardBoard[y, x].GetComponent<Image>().raycastTarget = false;

                    Image img = board[y, x].GetComponent<Image>();

                    int y1 = y;
                    int x1 = x;

                    Sprite sprite = sprites[0];

                    Button button = board[y, x].AddComponent<Button>();
                    button.onClick.AddListener(() =>
                    {
                        ChessEngine.boardRenderer.UpdateBoard();
                        for (int y2 = 0; y2 < 8; y2++)
                        {
                            for (int x2 = 0; x2 < 8; x2++)
                            {
                                MoveBoard[y2, x2].SetActive(false);
                            }
                        }
                        byte pieceType = HelperFunctions.GetBaseType(y1 * size + x1);


                        if ((ChessEngine.WhiteToMove && pieceType > 5) && ChessEngine.EnableAI)
                        {

                        }
                        else
                        {
                            


                            if (pieceType != byte.MaxValue)
                            {

                                MovePieces.Move[] moves = ChessEngine.Mover.GetLegalMoves(ref ChessEngine.board, pieceType, (byte)(y1 * size + x1));

                                for (int i1 = 0; i1 < moves.Length; i1++)
                                {
                                    int i = i1;

                                    MoveBoard[(moves[i].endPos / size), moves[i].endPos % size].SetActive(true);

                                    MoveBoard[(moves[i].endPos / size), moves[i].endPos % size].GetComponent<Button>().onClick.RemoveAllListeners();
                                    MoveBoard[(moves[i].endPos / size), moves[i].endPos % size].GetComponent<Button>().onClick.AddListener(() =>
                                    {


                                        ChessEngine.Mover.MovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType), pieceType, moves[i].startPos, moves[i].endPos);
                                        for (int y2 = 0; y2 < 8; y2++)
                                        {
                                            for (int x2 = 0; x2 < 8; x2++)
                                            {
                                                MoveBoard[y2, x2].SetActive(false);
                                            }
                                        }
                                        ChessEngine.boardRenderer.UpdateBoard();
                                        ChessEngine.board.UpdateBitBoards();

                                        if (ChessEngine.EnableAI)
                                        {
                                            ChessEngine.Mover.MakeAIMove(false);
                                        }
                                    });
                                }
                            }
                        }
                    });

                    MoveBoard[y,x].SetActive(false);
                    img.sprite = sprite;
                }
            }
        }

        public void UpdateBoard()
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    board[y, x].SetActive (true);
                    if (HelperFunctions.GetByte(y*size+x, ChessEngine.board.WhitePawns) == 1)
                    {
                        board[y,x].GetComponent<Image>().sprite = spriteSheet[0];
                    }
                    else if (HelperFunctions.GetByte(y*size+x, ChessEngine.board.WhiteRooks) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[1];
                    }
                    else if (HelperFunctions.GetByte(y * size + x, ChessEngine.board.WhiteBishops) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[2];
                    }
                    else if (HelperFunctions.GetByte(y * size + x, ChessEngine.board.WhiteKnights) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[3];
                    }
                    else if (HelperFunctions.GetByte(y * size + x, ChessEngine.board.WhiteQueens) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[4];
                    }
                    else if (HelperFunctions.GetByte(y * size + x, ChessEngine.board.WhiteKing) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[5];
                    }
                    else if (HelperFunctions.GetByte(y * size + x, ChessEngine.board.BlackPawns) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[6];
                    }
                    else if (HelperFunctions.GetByte(y * size + x, ChessEngine.board.BlackRooks) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[7];
                    }
                    else if (HelperFunctions.GetByte(y * size + x, ChessEngine.board.BlackBishops) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[8];
                    }
                    else if (HelperFunctions.GetByte(y * size + x, ChessEngine.board.BlackKnights) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[9];
                    }
                    else if (HelperFunctions.GetByte(y * size + x, ChessEngine.board.BlackQueens) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[10];
                    }
                    else if (HelperFunctions.GetByte(y * size + x, ChessEngine.board.BlackKing) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[11];
                    }
                    else
                    {
                        board[y,x].SetActive(false);
                    }

                    if (ChessEngine.board.WhiteAttackBoard[y * 8 + x])
                    {
                        attackBoardBoard[y, x].GetComponent<Image>().color = new Color(1, 0, 0, 0.3f);
                    }
                    else
                    {
                        attackBoardBoard[y, x].GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    }
                }
            }
        }
    }
}