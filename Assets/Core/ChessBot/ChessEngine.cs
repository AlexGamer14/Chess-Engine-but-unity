using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;

namespace ChessEngine
{
    public class ChessEngine : MonoBehaviour
    {
        [SerializeField, Tooltip("Defines the parents of the chess pieces")] Transform parentPanel;
        [SerializeField] Sprite[] sprites;
        [SerializeField] GameObject prefab;
        [SerializeField] GameObject movePrefab;
        [SerializeField] GameObject AttackBoardPrefab;

        public static ChessBoard board;
        public static ChessBoardRenderer boardRenderer;

        

        public static MovePieces Mover;
        public static Evaluation evaluation = new Evaluation();
        public static Search search = new Search();

        public static bool EnableAI = true;
        [SerializeField] bool EnableAIInspector = true;



        public static int MoveCount = 0;

        int[] pawnBonus = new int[64] { 0, 0, 0, 0, 0, 0,  0, 0,
                                                                10, 15, 5, 5,  5,  5, 15, 10,
                                                                5,  0,  5, 15, 15, 5, 0,  5,
                                                                0,  0,  10, 25, 25, 10, 0,  0,
                                                                0,  0,  0, 0,  0,  0, 0,  0,
                                                                0,  0,  0, 0,  0,  0, 0,  0,
                                                                0,  0,  0, 0,  0,  0, 0,  0,
                                                                0,  0,  0, 0,  0,  0, 0,  0,
                                                              } ;

        // There are 10 types of people in the world, those who understand binary and those who don't
        // There are 10 types of people in the world, those who understand trinary, those who think it is binary, and those who don't
        // There are 10 types of people in the world, those who understand quadary, those who think it is trinary, those who think it is binary, and those who don't
        // There are 10 types of people in the world, those who understand quinary, those who think it is quadrary, those who think it is trinary, those who think it is binary and those who don't.
        // There are 10 types of people in the world, those who understand seximal, those who think it is quinary, those who think it is quadrary, those who think it is trinary, those who think it is binary and those who don't.
        // There are 10 types of people in the world, those who understand septimal, those who think it is seximal, those who think it is quinary, those who think it is quadrary, those who think it is trinary, those who think it is binary and those who don't.
        // There are 10 types of people in the world, those who understand octal, those who think it is septimal, those who think it is seximal, those who think it is quinary, those who think it is quadrary, those who think it is trinary, those who think it is binary and those who don't.
        // There are 10 types of people in the world, those who understand nonal, those who think it is octal, those who think it is septimal, those who think it is seximal, those who think it is quinary, those who think it is quadrary, those who think it is trinary, those who think it is binary and those who don't.
        // There are 10 types of people in the world, those who understand decimal, those who think it is nonal, those who think it is octal, those who think it is septimal, those who think it is seximal, those who think it is quinary, those who think it is quadrary, those who think it is trinary, those who think it is binary and those who don't.
        // There are 10 types of people in the world, those who underestand undecimal, those who think it is decimal, those who think it is nonal, those who think it is octal, those who think it is septimal, those who think it is seximal, those who think it is quinary, those who think it is quadrary, those who think it is trinary, those who think it is binary and those who don't.
        // There are 10 types of people in the world, those who understand duodecimal, those who think it is undecimal, those who think it is decimal, those who think it is nonal, those who think it is octal, those who think it is septimal, those who think it is seximal, those who think it is quinary, those who think it is quadrary, those who think it is trinary, those who think it is binary and those who don't.
        // There are 10 types of people in the world, those who understand tridecimal, those who think it is duodecimal, those who think it is undecimal, those who think it is decimal, those who think it is nonal, those who think it is octal, those who think it is septimal, those who think it is seximal, those who think it is quinary, those who think it is quadrary, those who think it is trinary, those who think it is binary and those who don't.
        // There are 10 types of people in the world, those who understand quattuordecimal, those who think it is tridecimal, those who think it is duodecimal, those who think it is undecimal, those who think it is decimal, those who think it is nonal, those who think it is octal, those who think it is septimal, those who think it is seximal, those who think it is quinary, those who think it is quadrary, those who think it is trinary, those who think it is binary and those who don't.
        // There are 10 types of people in the world, those who understand pentadecimal, those who think it is quattuordecimal, those who think it is tridecimal, those who think it is duodecimal, those who think it is undecimal, those who think it is decimal, those who think it is nonal, those who think it is octal, those who think it is septimal, those who think it is seximal, those who think it is quinary, those who think it is quadrary, those who think it is trinary, those who think it is binary and those who don't.
        // There are 10 types of people in the world, those who understand hexadecimal, those who think it is pentadecimal, those who think it is quattuordecimal, those who think it is tridecimal, those who think it is duodecimal, those who think it is undecimal, those who think it is decimal, those who think it is nonal, those who think it is octal, those who think it is septimal, those who think it is seximal, those who think it is quinary, those who think it is quadrary, those who think it is trinary, those who think it is binary and those who don't.


        public static float FiftyMoveRule = 0;

        public float cooldown = 0.1f;

        private float timer = 0;

        public void Awake()
        {
            //board = new ChessBoard();
            board = LoadFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            //Console.WriteLine(GetByte(1, board.AllPieces));

            EnableAI = EnableAIInspector;

            Debug.Log("Chess engine is running");

            Mover = new();

            boardRenderer = new ChessBoardRenderer();
            boardRenderer.Initialize(parentPanel, sprites, prefab, movePrefab: movePrefab, AttackBoardPrefab);

            boardRenderer.UpdateBoard();
        }

        

        private void Update()
        {
            if (Input.GetKey(KeyCode.E) && timer > cooldown)
            {
                Mover.GetMovesForBlackOrWhite(true, board);
                boardRenderer.UpdateAttackBoard(true);
            }
            if (Input.GetKey(KeyCode.R) && timer > cooldown)
            {
                Mover.GetMovesForBlackOrWhite(false, board);
                boardRenderer.UpdateAttackBoard(false);
            }
            timer += Time.deltaTime;
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                board.PrintBoard();
            }
        }
    
    public ChessBoard LoadFenString(string FenString) {
            // rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
            /*byte fenPosition = 0;
            byte piecePosition = 0;
            bool isWhiteToMove;*/

            FenString = FenString.Replace("/", "");

            ChessBoard board = new();
            board.ClearBoard();

            // Piece positions
            int piecePosition = 0;
            for (int i = 0; i < FenString.Length; i++) {
                if (FenString[i] == ' ') {
                    break;
                }

                // Check for black pieces
                if (FenString[i] == 'r') {
                    HelperFunctions.SetBit(ref board.BlackRooks, piecePosition, 1);
                } else if (FenString[i] == 'n') {
                    HelperFunctions.SetBit(ref board.BlackKnights, piecePosition, 1);
                } else if (FenString[i] == 'b') {
                    HelperFunctions.SetBit(ref board.BlackBishops, piecePosition, 1);
                } else if (FenString[i] == 'k') {
                    HelperFunctions.SetBit(ref board.BlackKing, piecePosition, 1);
                } else if (FenString[i] == 'q') {
                    HelperFunctions.SetBit(ref board.BlackQueens, piecePosition, 1);
                } else if (FenString[i] == 'p') {
                    HelperFunctions.SetBit(ref board.BlackPawns, piecePosition, 1);
                }

                // Check for white pieces
                if (FenString[i] == 'R') {
                    HelperFunctions.SetBit(ref board.WhiteRooks, piecePosition, 1);
                } else if (FenString[i] == 'N') {
                    HelperFunctions.SetBit(ref board.WhiteKnights, piecePosition, 1);
                } else if (FenString[i] == 'B') {
                    HelperFunctions.SetBit(ref board.WhiteBishops, piecePosition, 1);
                } else if (FenString[i] == 'K') {
                    HelperFunctions.SetBit(ref board.WhiteKing, piecePosition, 1);
                } else if (FenString[i] == 'Q') {
                    HelperFunctions.SetBit(ref board.WhiteQueens, piecePosition, 1);
                } else if (FenString[i] == 'P') {
                    HelperFunctions.SetBit(ref board.WhitePawns, piecePosition, 1);
                }

                // Make numbers in fenstrings work
                // NOTE: I'm not adding cases for the number 1 because it is already done by the for loop
                if (FenString[i] == '2') {
                    piecePosition++;
                } else if (FenString[i] == '3') {
                    piecePosition += 2;
                } else if (FenString[i] == '4') {
                    piecePosition += 3;
                } else if (FenString[i] == '5') {
                    piecePosition += 4;
                } else if (FenString[i] == '6') {
                    piecePosition += 5;
                } else if (FenString[i] == '7') {
                    piecePosition += 6;
                } else if (FenString[i] == '8') {
                    piecePosition += 7;
                }

                piecePosition ++;
            }

            // Side to move
            /*for (int i = 0; i<= FenString.Length, i++) {
                if (FenString[i] == ' ') {
                    if (FenString[i + 1] == 'w') {
                        isWhiteToMove = true;
                    } else if (FenString[i + 1] = 'b') {
                        isWhiteToMove = false;
                    } else {
                        Debug.Log("Insert a valid FEN-string");
                    }
                }
            }

            // Castling
        */
            //finish setting up the board
            board.WhiteBishops = HelperFunctions.FlipBitboard(board.WhiteBishops);
            board.WhiteKing = HelperFunctions.FlipBitboard(board.WhiteKing);
            board.WhiteQueens = HelperFunctions.FlipBitboard(board.WhiteQueens);
            board.WhiteRooks = HelperFunctions.FlipBitboard(board.WhiteRooks);
            board.WhiteKnights = HelperFunctions.FlipBitboard(board.WhiteKnights);
            board.WhitePawns = HelperFunctions.FlipBitboard(board.WhitePawns);

            board.BlackBishops = HelperFunctions.FlipBitboard(board.BlackBishops);
            board.BlackKing = HelperFunctions.FlipBitboard(board.BlackKing);
            board.BlackQueens = HelperFunctions.FlipBitboard(board.BlackQueens);
            board.BlackRooks = HelperFunctions.FlipBitboard(board.BlackRooks);
            board.BlackKnights = HelperFunctions.FlipBitboard(board.BlackKnights);
            board.BlackPawns = HelperFunctions.FlipBitboard(board.BlackPawns);

            board.SetUpAllBoards();
            return(board);
        }

        /*
        Fenpos
        Piecepos
        EndOfPos
        Loop through FenString
            Check if char at index of FenString = space
                If so set endOfPos(end of part of fen string containing info about piece positions)
                break
            set bitboard of piece type to include that piece type at bit position

        Loop through FenString starting at EnOfPos ending either "w" or "b"
            Check if char at index of FenString = "w"|"b"
                If so set start of castlingInfo to index+1

        Loop through FenString starting at castlingInfo
            Set castling to valid castling based on fen string('-' | ['K'] ['Q'] ['k'] ['q'] (1..4))
        Go to end of    
        */
    }
}