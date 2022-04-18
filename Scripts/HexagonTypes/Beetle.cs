using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beetle : Hexagon
{
    public void SelectBlackBeetle()
    {
        if (GameManager.Instance.turn == Turn.White) return;
        Select(HexagonColor.Black, blackSprite, Type.Beetle);
    }
    
    public void SelectWhiteBeetle()
    {
        if (GameManager.Instance.turn == Turn.Black) return;
        Select(HexagonColor.White, whiteSprite, Type.Beetle);
    }
}
