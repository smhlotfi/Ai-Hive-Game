using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlack : MonoBehaviour
{
    public int beeCount;
    public int antCount;
    public int beetleCount;
    public int grassHopperCount;
    public int spiderCount;
    
    public int moveNumber = 1;
    
    
    public void DecreaseCount(Type type)
    {
        switch (type)
        {
            case Type.Ant:
                antCount--;
                break;
            case Type.Bee:
                beeCount--;
                break;
            case Type.Beetle:
                beetleCount--;
                break;
            case Type.GrassHopper:
                grassHopperCount--;
                break;
            case Type.Spider:
                spiderCount--;
                break;
        }
    }

    public bool IsZeroCount(Type type)
    {
        switch (type)
        {
            case Type.Ant:
                return antCount == 0;
            case Type.Bee:
                return beeCount == 0;
            case Type.Beetle:
                return beetleCount == 0;
            case Type.GrassHopper:
                return grassHopperCount == 0;
            case Type.Spider:
                return spiderCount == 0;
        }

        return false;
    }
    
    public bool IsBeeUsed()
    {
        return beeCount == 0;
    }

    public int GetUsedTowsCount()
    {
        int usedCount = 0;
        usedCount += 3 - antCount;
        usedCount += 1 - beeCount;
        usedCount += 2 - beetleCount;
        usedCount += 2 - spiderCount;
        usedCount += 3 - grassHopperCount;
        return usedCount;
    }
    
    
    
    
    
    
    private PlayerColor _color;
    

    public bool IsTurn()
    {
        return _color == PlayerColor.Black && GameManager.Instance.turn == Turn.Black || 
               _color == PlayerColor.White && GameManager.Instance.turn == Turn.White;
    }
}

public enum PlayerColor
{
    Black, White
}
