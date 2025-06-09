using System;
using System.Collections;
using System.Collections.Generic;
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


        public Button queenPromoteBtn;
        public Button rookPromoteBtn;
        public Button bishopPromoteBtn;
        public Button knightPromoteBtn;

        public GameObject promotionBackground;

        private Image evalImage;


        public void Initialize(Transform parentObj, Sprite[] sprites, GameObject prefab, GameObject movePrefab, GameObject AttackBoardPrefab, GameObject qpbtn, GameObject rpbtn, GameObject bpbtn, GameObject kpbtn, GameObject backgroundPromotion)
        {
            board = new GameObject[size, size];
            MoveBoard = new GameObject[size, size];
            attackBoardBoard = new GameObject[size, size];
            spriteSheet = sprites;

            queenPromoteBtn = qpbtn.GetComponent<Button>();
            rookPromoteBtn = rpbtn.GetComponent<Button>();
            bishopPromoteBtn = bpbtn.GetComponent<Button>();
            knightPromoteBtn = kpbtn.GetComponent<Button>();

            evalImage = GameObject.Find("EvalImage").GetComponent<Image>();

            if (queenPromoteBtn is null || rookPromoteBtn is null || bishopPromoteBtn is null || knightPromoteBtn is null)
            {
                Debug.Log("One of the buttons are null");
            }

            promotionBackground = backgroundPromotion;

            promotionBackground.SetActive(false);

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


                        if ((pieceType < 6 || !ChessEngine.EnableAI))
                        {

                            if (pieceType != byte.MaxValue)
                            {
                                MovePieces.Move[] moves = MovePieces.GetLegalMoves(ref ChessEngine.board, pieceType, (byte)(y1 * size + x1));

                                for (int i1 = 0; i1 < moves.Length; i1++)
                                {
                                    int i = i1;


                                    MoveBoard[(moves[i].endPos / size), moves[i].endPos % size].SetActive(true);

                                    MoveBoard[(moves[i].endPos / size), moves[i].endPos % size].GetComponent<Button>().onClick.RemoveAllListeners();
                                    MoveBoard[(moves[i].endPos / size), moves[i].endPos % size].GetComponent<Button>().onClick.AddListener(() =>
                                    { 
                                        MovePieces.Move currentMove = moves[i];

                                        if (moves[i].specialFlags ==MovePieces.SpecialFlags.Knight)
                                        {
                                            promotionBackground.SetActive(true);

                                            // Adding promotion buttons
                                            MovePieces.Move knightMove = moves[i];
                                            MovePieces.Move bishopMove = moves[i - 1];
                                            MovePieces.Move rookMove = moves[i - 2];
                                            MovePieces.Move queenMove = moves[i - 3];

                                            queenPromoteBtn.onClick.RemoveAllListeners();
                                            queenPromoteBtn.onClick.AddListener(() =>
                                            {
                                                SecondPartOfHumanMove(queenMove, pieceType);
                                                promotionBackground.SetActive(false);
                                            });
                                            rookPromoteBtn.onClick.RemoveAllListeners();
                                            rookPromoteBtn.onClick.AddListener(() =>
                                            {
                                                SecondPartOfHumanMove(rookMove, pieceType);
                                                promotionBackground.SetActive(false);
                                            });
                                            bishopPromoteBtn.onClick.RemoveAllListeners();
                                            bishopPromoteBtn.onClick.AddListener(() =>
                                            {
                                                SecondPartOfHumanMove(bishopMove, pieceType);
                                                promotionBackground.SetActive(false);
                                            });
                                            knightPromoteBtn.onClick.RemoveAllListeners();
                                            knightPromoteBtn.onClick.AddListener(() =>
                                            {
                                                SecondPartOfHumanMove(knightMove, pieceType);
                                                promotionBackground.SetActive(false);
                                            });

                                            return;
                                        }
                                        SecondPartOfHumanMove(currentMove, pieceType);
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

        public void UpdateEval()
        {
            Evaluation evaluation = new Evaluation();
            evalImage.fillAmount = (evaluation.Evaluate(ChessEngine.board, ChessEngine.board.WhiteToMove) / 2000f) + 0.5f;
        }

        public void SecondPartOfHumanMove(MovePieces.Move move, int pieceType)
        {
            MovePieces.MovePiece(ref HelperFunctions.GetTypeBasedOnIndex(pieceType), pieceType, move, ref ChessEngine.board);
            ChessEngine.prevMove = move;

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
                ChessEngine.MoveBlackAI = true;
            }
        }

        public void UpdateBoard()
        {
            UpdateEval();

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    board[y, x].SetActive (true);
                    if (HelperFunctions.GetBit(y*size+x, ChessEngine.board.WhitePawns) == 1)
                    {
                        board[y,x].GetComponent<Image>().sprite = spriteSheet[0];
                    }
                    else if (HelperFunctions.GetBit(y*size+x, ChessEngine.board.WhiteRooks) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[1];
                    }
                    else if (HelperFunctions.GetBit(y * size + x, ChessEngine.board.WhiteBishops) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[2];
                    }
                    else if (HelperFunctions.GetBit(y * size + x, ChessEngine.board.WhiteKnights) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[3];
                    }
                    else if (HelperFunctions.GetBit(y * size + x, ChessEngine.board.WhiteQueens) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[4];
                    }
                    else if (HelperFunctions.GetBit(y * size + x, ChessEngine.board.WhiteKing) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[5];
                    }
                    else if (HelperFunctions.GetBit(y * size + x, ChessEngine.board.BlackPawns) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[6];
                    }
                    else if (HelperFunctions.GetBit(y * size + x, ChessEngine.board.BlackRooks) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[7];
                    }
                    else if (HelperFunctions.GetBit(y * size + x, ChessEngine.board.BlackBishops) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[8];
                    }
                    else if (HelperFunctions.GetBit(y * size + x, ChessEngine.board.BlackKnights) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[9];
                    }
                    else if (HelperFunctions.GetBit(y * size + x, ChessEngine.board.BlackQueens) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[10];
                    }
                    else if (HelperFunctions.GetBit(y * size + x, ChessEngine.board.BlackKing) == 1)
                    {
                        board[y, x].GetComponent<Image>().sprite = spriteSheet[11];
                    }
                    else
                    {
                        board[y,x].SetActive(false);
                    }
                }

            }
        }

        public IEnumerator WaitUntilCondition(bool condition)
        {
            yield return new WaitUntil(() => condition == true);
        }

        public void UpdateAttackBoard(bool white, bool reset_val=true)
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (reset_val) attackBoardBoard[y, x].GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    if (HelperFunctions.GetBit(y * 8 + x, ChessEngine.board.WhiteAttackBoard)==1 && white)
                    {
                        attackBoardBoard[y, x].GetComponent<Image>().color = new Color(1, 0, 0, 0.3f);
                    }
                    else if (white)
                    {
                        attackBoardBoard[y, x].GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    }

                    if (HelperFunctions.GetBit(y * 8 + x, ChessEngine.board.BlackAttackBoard) == 1 && !white)
                    {
                        attackBoardBoard[y, x].GetComponent<Image>().color = new Color(1, 0, 0, 0.3f);
                    }
                    else if (!white)
                    {
                        attackBoardBoard[y, x].GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    }
                }

            }
        }
    }
}