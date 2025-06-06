using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugVariable : MonoBehaviour
{
    public TMPro.TextMeshProUGUI CastleKingText;
    public TMPro.TextMeshProUGUI CastleQueenText;
    public TMPro.TextMeshProUGUI BlackKingCastleText;
    public TMPro.TextMeshProUGUI BlackQueenCastleText;

    private void Awake()
    {
        if (CastleKingText == null || CastleQueenText == null || BlackKingCastleText == null || BlackQueenCastleText == null)
        {
            Debug.LogError("DebugVariable: One or more TextMeshProUGUI references are not assigned.");
        }
    }

    private void Update()
    {
        CastleKingText.text = "White Castle King: " + ChessEngine.ChessEngine.board.WhiteCanCastleKingside.ToString();
        CastleQueenText.text = "White Castle Queen: " + ChessEngine.ChessEngine.board.WhiteCanCastleQueenside.ToString();
        BlackKingCastleText.text = "Black Castle King: " + ChessEngine.ChessEngine.board.BlackCanCastleKingside.ToString();
        BlackQueenCastleText.text = "Black Castle Queen: " + ChessEngine.ChessEngine.board.BlackCanCastleQueenside.ToString();
    }
}
