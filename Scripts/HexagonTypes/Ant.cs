using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Ant : Hexagon
{
    public void SelectBlackAnt()
    {
        if (GameManager.Instance.turn == Turn.White) return;
        Select(HexagonColor.Black, blackSprite, Type.Ant);
    }
    
    public void SelectWhiteAnt()
    {
        if (GameManager.Instance.turn == Turn.Black) return;
        Select(HexagonColor.White, whiteSprite, Type.Ant);
    }

    public static Dictionary<int, Hexagon> ShowMovementPossibilities()
    {
        var _hiveMargin = GameManager.Instance.hiveMargins;
        var _possibilities = new Dictionary<int, Hexagon>();
    }

    
}
