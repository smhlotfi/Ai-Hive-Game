using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    private HexagonColor _hexagonColor;
    private Type _type;

    public Hexagon up;
    public Hexagon down;
    public Hexagon upRight;
    public Hexagon upLeft;
    public Hexagon downRight;
    public Hexagon downLeft;

    private bool _isFreeze; // Beetle on that

    private GameObject Create(Hexagon creator, HexagonDirection direction, Vector2 pos, Transform parent)
    {
        var newHexagon = Instantiate(gameObject, pos, quaternion.identity, parent);
        return newHexagon;
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
