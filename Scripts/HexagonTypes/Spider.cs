using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Hexagon
{
    public void SelectBlackSpider()
    {
        if (GameManager.Instance.turn == Turn.White) return;
        Select(HexagonColor.Black, blackSprite, Type.Spider);
    }
    
    public void SelectWhiteSpider()
    {
        if (GameManager.Instance.turn == Turn.Black) return;
        Select(HexagonColor.White, whiteSprite, Type.Spider);
    }
}
