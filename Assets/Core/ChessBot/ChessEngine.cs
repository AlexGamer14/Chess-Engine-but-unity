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
                Mover.MakeAIMove(ChessEngine.WhiteToMove);
                timer = 0;
            }
            timer += Time.deltaTime;
        }
    }
}