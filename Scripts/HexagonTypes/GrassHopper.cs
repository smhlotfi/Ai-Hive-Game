using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassHopper : Hexagon
{
    public void SelectBlackGrassHopper()
    {
        if (GameManager.Instance.turn == Turn.White) return;
        Select(HexagonColor.Black, blackSprite, Type.GrassHopper);
    }
    
    public void SelectWhiteGrassHopper()
    {
        if (GameManager.Instance.turn == Turn.Black) return;
        Select(HexagonColor.White, whiteSprite, Type.GrassHopper);
    }
}
