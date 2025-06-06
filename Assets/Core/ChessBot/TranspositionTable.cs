using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UI;

namespace ChessEngine
{
    public class TranspositionTable
    {
        public enum NodeType
        {
            Exact,
            LowerBound,
            UpperBound
        }

        private readonly TTEntry[] table;
        private readonly int mask;

        public TranspositionTable(int sizePowerOfTwo = 20) // 2^20 = ~1M entries
        {
            int size = 1 << sizePowerOfTwo;
            table = new TTEntry[size];
            mask = size - 1;
        }

        private int Index(ulong zobristKey) => (int)(zobristKey & (ulong)mask);

        public void Store(ulong key, float eval, int depth, NodeType type, MovePieces.Move bestMove)
        {
            int idx = Index(key);
            var current = table[idx];

            // Replace if new entry is deeper
            if (current.ZobristKey != key || depth >= current.Depth)
            {
                table[idx] = new TTEntry(key, eval, depth, type, bestMove);
            }
        }

        public bool TryGet(ulong key, out TTEntry entry)
        {
            entry = table[Index(key)];
            return entry.ZobristKey == key;
        }

        public void Clear()
        {
            Array.Clear(table, 0, table.Length);
        }

        public struct TTEntry
        {
            public ulong ZobristKey;
            public float Evaluation;
            public int Depth;
            public NodeType Type;
            public MovePieces.Move BestMove;

            public TTEntry(ulong key, float eval, int depth, NodeType type, MovePieces.Move move)
            {
                ZobristKey = key;
                Evaluation = eval;
                Depth = depth;
                Type = type;
                BestMove = move;
            }
        }
    }
}