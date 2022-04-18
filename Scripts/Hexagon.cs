using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    public int id;
    public HexagonColor _color;
    public Type _type;

    public Hexagon up;
    public Hexagon down;
    public Hexagon upRight;
    public Hexagon upLeft;
    public Hexagon downRight;
    public Hexagon downLeft;

    public bool isPossibility;

    private bool _isFreeze; // Beetle on that

    

    public bool IsEmpty()
    {
        return _type == Type.Empty;
    }
    

}

public enum HexagonColor
{
    Black, White
}
public enum Type
{
    Empty, Bee, Beetle, GrassHopper, Ant, Spider
}

public enum HexagonDirection
{
    Up, UpRight, DownRight, Down, DownLeft, UpLeft
}
