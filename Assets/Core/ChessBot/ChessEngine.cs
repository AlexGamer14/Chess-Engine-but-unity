using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace ChessEngine
{
    public class ChessEngine : MonoBehaviour
    {
        [SerializeField, Tooltip("Defines the parents of the chess pieces")] Transform parentPanel;
        [SerializeField] Sprite[] sprites;
        [SerializeField] GameObject prefab;
        [SerializeField] GameObject movePrefab;

        public static ChessBoard board;
        public static ChessBoardRenderer boardRenderer;

        public static bool WhiteToMove = true;

        public static MovePieces Mover;
        public static Evaluation evaluation = new Evaluation();
        public static Search search = new Search();

        public static int[] WhiteAttackBoard;

        public static int[] BlackAttackBoard;
        public static bool EnableAI = true;
        [SerializeField] bool EnableAIInspector = true;

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

        [SerializeField] public static byte MovedTwoSpacesLastTurn;

        public float cooldown = 0.1f;

        private float timer = 0;

        public void Awake()
        {
            board = new ChessBoard();
            //Console.WriteLine(GetByte(1, board.AllPieces));

            EnableAI = EnableAIInspector;

            Debug.Log("Chess engine is running");

            Mover = new();
            
            WhiteAttackBoard = new();
            BlackAttackBoard = new();

            boardRenderer = new ChessBoardRenderer();
            boardRenderer.Initialize(parentPanel, sprites, prefab, movePrefab: movePrefab);

            boardRenderer.UpdateBoard();
        }

        

        private void Update()
        {
            if (Input.GetKey(KeyCode.E) && timer > cooldown)
            {
                Debug.Log(MovedTwoSpacesLastTurn);
            }
            timer += Time.deltaTime;
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log(Mover.GetMovesForBlackOrWhite(true, board).Length) ;
            }
        }
    
    /*public LoadFenString(string FenString) = new() {
            // rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
            byte fenPosition = 0;
            byte piecePosition = 0;
            bool isWhiteToMove;
            // Piece positions
            for (int i = 0; i < FenString.Length, i++) {
                if (FenString[i] = " ") {
                    break;
                }

                // TODO: Find piece type, and convert fenPosition to piecePosition and set bitboard to that value
            }

            // Side to move
            for (int i = 0; i<= FenString.Length, i++) {
                if (FenString[i] = " ") {
                    if (FenString[i + 1] = "w") {
                        isWhiteToMove = true;
                    } else if (FenString[i + 1] = "b") {
                        isWhiteToMove = false;
                    } else {
                        Debug.Log("Insert a valid FEN-string");
                    }
                }
            }

            // Castling

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