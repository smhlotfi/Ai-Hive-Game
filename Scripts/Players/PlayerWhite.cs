using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerWhite : MonoBehaviour
{
    public int beeCount;
    public int antCount;
    public int beetleCount;
    public int grassHopperCount;
    public int spiderCount;

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
}
