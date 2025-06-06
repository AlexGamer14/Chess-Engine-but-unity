using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace ChessEngine
{
    public class ChessEngine : MonoBehaviour
    {
        [SerializeField, Tooltip("Defines the parents of the chess pieces")] Transform parentPanel;
        [SerializeField] Sprite[] sprites;
        [SerializeField] GameObject prefab;
        [SerializeField] GameObject movePrefab;
        [SerializeField] GameObject AttackBoardPrefab;


        public GameObject queenPromoteBtn;
        public GameObject rookPromoteBtn;
        public GameObject bishopPromoteBtn;
        public GameObject knightPromoteBtn;

        public GameObject promotionBackground;

        public static ChessBoard board;
        public static ChessBoardRenderer boardRenderer;
        public static Evaluation evaluation = new Evaluation();

        public static bool EnableAI = true;
        [SerializeField] bool EnableAIInspector = true;

        public InputField FENInput;

        public int depth_inspector = 3;

        public static int depth;

        public static Transform self;

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



        public float cooldown = 0.1f;

        public static MovePieces.Move prevMove = new();

        private float timer = 0;

        public void Awake()
        {

            depth = depth_inspector;
            self = transform;

            //board = new ChessBoard();
            //Console.WriteLine(GetByte(1, board.AllPieces));

            EnableAI = EnableAIInspector;

            Debug.Log("Chess engine is running");

            LoadFenString();

            boardRenderer = new ChessBoardRenderer();
            boardRenderer.Initialize(parentPanel, sprites, prefab, movePrefab: movePrefab, AttackBoardPrefab, queenPromoteBtn, rookPromoteBtn, bishopPromoteBtn, knightPromoteBtn, promotionBackground);

            boardRenderer.UpdateBoard();
            boardRenderer.UpdateAttackBoard(true,true);
            MovePieces.UpdateAttackBoard(ref board);

            Debug.Log(System.Runtime.InteropServices.Marshal.SizeOf<TranspositionTable.TTEntry>());
        }



        private void Update()
        {
            depth = depth_inspector;
            if (Input.GetKey(KeyCode.E) && timer > cooldown)
            {
                boardRenderer.UpdateAttackBoard(true);
            }
            if (Input.GetKey(KeyCode.R) && timer > cooldown)
            {
                boardRenderer.UpdateAttackBoard(false);
            }
            timer += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.F))
            {
                print("Black is " + (board.IsBlackChecked() ? "" : "not ") + "checked");
                print("White is " + (board.IsWhiteChecked() ? "" : "not ") + "checked");
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                HelperFunctions.PrintArray(board.WhiteAttackBoard);
            }
        }

        public void LoadFenString()
        {
            // rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
            string fenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            board = new();


            byte piecePosition = 0;
            fenString = fenString.Replace("/", "");

            // Find location of spaces
            List<int> spaceIndices = new List<int>();

            for (int j = 0; j < fenString.Length; j++)
            {
                if (fenString[j] == ' ')
                {
                    spaceIndices.Add(j);
                }
            }

            int[] spaceIndicesArray = spaceIndices.ToArray();

            /*for (int k = 0; k < spaceIndicesArray.Length; k++) {
                Debug.Log(spaceIndicesArray[k]);
            }*/

            board.ClearBoard();
            int i;

            // Piece positions
            for (i = 0; i < fenString.Length; i++)
            {
                if (fenString[i] == ' ')
                {
                    break;
                }

                // Check for black pieces
                if (fenString[i] == 'r')
                {
                    HelperFunctions.SetBit(ref board.BlackRooks, piecePosition, 1);
                }
                else if (fenString[i] == 'n')
                {
                    HelperFunctions.SetBit(ref board.BlackKnights, piecePosition, 1);
                }
                else if (fenString[i] == 'b')
                {
                    HelperFunctions.SetBit(ref board.BlackBishops, piecePosition, 1);
                }
                else if (fenString[i] == 'k')
                {
                    HelperFunctions.SetBit(ref board.BlackKing, piecePosition, 1);
                }
                else if (fenString[i] == 'q')
                {
                    HelperFunctions.SetBit(ref board.BlackQueens, piecePosition, 1);
                }
                else if (fenString[i] == 'p')
                {
                    HelperFunctions.SetBit(ref board.BlackPawns, piecePosition, 1);
                }

                // Check for white pieces
                if (fenString[i] == 'R')
                {
                    HelperFunctions.SetBit(ref board.WhiteRooks, piecePosition, 1);
                }
                else if (fenString[i] == 'N')
                {
                    HelperFunctions.SetBit(ref board.WhiteKnights, piecePosition, 1);
                }
                else if (fenString[i] == 'B')
                {
                    HelperFunctions.SetBit(ref board.WhiteBishops, piecePosition, 1);
                }
                else if (fenString[i] == 'K')
                {
                    HelperFunctions.SetBit(ref board.WhiteKing, piecePosition, 1);
                }
                else if (fenString[i] == 'Q')
                {
                    HelperFunctions.SetBit(ref board.WhiteQueens, piecePosition, 1);
                }
                else if (fenString[i] == 'P')
                {
                    HelperFunctions.SetBit(ref board.WhitePawns, piecePosition, 1);
                }

                // Make numbers in fenStrings work
                // NOTE: I'm not adding cases for the number 1 because it is already done by the for loop
                if (fenString[i] == '2')
                {
                    piecePosition++;
                }
                else if (fenString[i] == '3')
                {
                    piecePosition += 2;
                }
                else if (fenString[i] == '4')
                {
                    piecePosition += 3;
                }
                else if (fenString[i] == '5')
                {
                    piecePosition += 4;
                }
                else if (fenString[i] == '6')
                {
                    piecePosition += 5;
                }
                else if (fenString[i] == '7')
                {
                    piecePosition += 6;
                }
                else if (fenString[i] == '8')
                {
                    piecePosition += 7;
                }

                piecePosition++;
            }

            // Side to move
            if (fenString[spaceIndicesArray[0] + 1] == 'w')
            {
                board.WhiteToMove = true;
            }
            else
            {
                board.WhiteToMove = false;
            }

            // Castling
            FenStringCastling(fenString, spaceIndicesArray[1]);
            
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

            board.UpdateBitBoards();

            ChessEngine.board = (ChessBoard)board.Clone();
        }

        private void FenStringCastling(string fenString, int fenStringPos)
        {
            Debug.Log($"Fenstring: {fenString}, Fenstringposition: {fenStringPos}");
            // First position
            if (fenString[fenStringPos + 1] == '-')
            {
                board.WhiteCanCastleKingside = false;
                board.WhiteCanCastleQueenside = false;
                board.BlackCanCastleKingside = false;
                board.BlackCanCastleQueenside = false;

                return;
            }
            else if (fenString[fenStringPos + 1] == 'K')
            {
                board.WhiteCanCastleKingside = true;
            }
            else if (fenString[fenStringPos + 1] == 'Q')
            {
                board.WhiteCanCastleQueenside = true;
            }
            else if (fenString[fenStringPos + 1] == 'k')
            {
                board.BlackCanCastleKingside = true;
            }
            else if (fenString[fenStringPos + 1] == 'q')
            {
                board.BlackCanCastleQueenside = true;

                return;
            }

            if (fenString[fenStringPos + 2] == ' ')
            {
                return;
            }
            else if (fenString[fenStringPos + 2] == 'Q')
            {
                board.WhiteCanCastleQueenside = true;
            }
            else if (fenString[fenStringPos + 2] == 'k')
            {
                board.BlackCanCastleKingside = true;
            }
            else if (fenString[fenStringPos + 2] == 'q')
            {
                board.BlackCanCastleQueenside = true;
                
                return;
            }

            if (fenString[fenStringPos + 3] == ' ')
            {
                return;
            }
            else if (fenString[fenStringPos + 3] == 'k')
            {
                board.BlackCanCastleKingside = true;
            }
            else if (fenString[fenStringPos + 3] == 'q')
            {
                board.BlackCanCastleQueenside = true;
                
                return;
            }

            if (fenString[fenStringPos + 4] == ' ')
            {
                return;
            }
            else if (fenString[fenStringPos + 4] == 'q')
            {
                board.BlackCanCastleQueenside = true;
                
                return;
            }


        }

        public void EditFEN()
        {
            string FENinput = FENInput.text;
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