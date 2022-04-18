using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : Hexagon
{
    public void SelectBlackBee()
    {
        if (GameManager.Instance.turn == Turn.White) return;
        Select(HexagonColor.Black, blackSprite, Type.Bee);
    }
    
    public void SelectWhiteBee()
    {
        if (GameManager.Instance.turn == Turn.Black) return;
        Select(HexagonColor.White, whiteSprite, Type.Bee);
    }
}
