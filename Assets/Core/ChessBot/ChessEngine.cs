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

        public static bool HasWhiteKingMoved = false;
        public static bool HasBlackKingMoved = false;

        public static bool[] HasRooksMoved;

        public static MovePieces Mover;
        public static Evaluation evaluation = new Evaluation();
        public static Search search = new Search();

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

            Debug.Log("Chess engine is running");

            Mover = new();

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
        }
    }
}