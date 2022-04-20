using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassHopper : Hexagon
{
    private static GrassHopper _instance;
    public static GrassHopper Instance => _instance ??= FindObjectOfType<GrassHopper>();
    
    public Dictionary<int, Hexagon> MovementPossibilities
    {
        get
        {
            return GameManager.Instance.movementPossibilities;
        }
        set
        {
            GameManager.Instance.movementPossibilities = value;
        }
    }
    
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
    
    public void Move(Hexagon src, Hexagon dest, Sprite sprite, HexagonColor color, Type type)
    {
        src.RemoveFromStack();
        src.GetComponent<SpriteRenderer>().sprite = idleSprite;
        src._color = HexagonColor.NotDefined;
        src._type = Type.Empty;

        dest.GetComponent<SpriteRenderer>().sprite = sprite;
        dest._color = color;
        dest._type = type;
        dest.AddToStack();

        GameManager.Instance.RemoveFilledHexagon(src);
        GameManager.Instance.AddFilledHexagon(dest);
        HideMovementPossibilities();
    }

    public void ShowMovementPossibilities(Hexagon clickedHexagon)
    {
        HideMovementPossibilities();
        MovementPossibilities = new Dictionary<int, Hexagon>();
        Hexagon possibilityHexagon;
        if (!clickedHexagon.up.IsEmpty())
        {
            possibilityHexagon = FindEmptyHexagon(clickedHexagon, Direction.Up);
            MovementPossibilities.Add(possibilityHexagon.id, possibilityHexagon);
        }
        if (!clickedHexagon.upRight.IsEmpty())
        {
            possibilityHexagon = FindEmptyHexagon(clickedHexagon, Direction.UpRight);
            MovementPossibilities.Add(possibilityHexagon.id, possibilityHexagon);
        }
        if (!clickedHexagon.downRight.IsEmpty())
        {
            possibilityHexagon = FindEmptyHexagon(clickedHexagon, Direction.DownRight);
            MovementPossibilities.Add(possibilityHexagon.id, possibilityHexagon);
        }
        if (!clickedHexagon.down.IsEmpty())
        {
            possibilityHexagon = FindEmptyHexagon(clickedHexagon, Direction.Down);
            MovementPossibilities.Add(possibilityHexagon.id, possibilityHexagon);
        }
        if (!clickedHexagon.downLeft.IsEmpty())
        {
            possibilityHexagon = FindEmptyHexagon(clickedHexagon, Direction.DownLeft);
            MovementPossibilities.Add(possibilityHexagon.id, possibilityHexagon);
        }
        if (!clickedHexagon.upLeft.IsEmpty())
        {
            possibilityHexagon = FindEmptyHexagon(clickedHexagon, Direction.UpLeft);
            MovementPossibilities.Add(possibilityHexagon.id, possibilityHexagon);
        }
        
        foreach (var possibility in MovementPossibilities)
        {
            var hexagon = possibility.Value;
            hexagon.isMovementPossibility = true;
            var pos = hexagon.transform.position;
            pos.z -= 0.01f;
            var rot = hexagon.transform.rotation;
            var possibilityObject = Instantiate(possibilityPrefab, pos, rot);
            GameManager.Instance.movementPossibilityPrefabs.Add(possibilityObject);
        }
    }

    private Hexagon FindEmptyHexagon(Hexagon start, Direction direction)
    {
        var lastHexagon = GetNextHexagonInDirection(start, direction);
        for (int i = 0; i < 23; i++)
        {
            if (lastHexagon.IsEmpty())
            {
                return lastHexagon;
            }
            lastHexagon = GetNextHexagonInDirection(lastHexagon, direction);
        }

        return lastHexagon;
    }

    private Hexagon GetNextHexagonInDirection(Hexagon lastHexagon, Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return lastHexagon.up;
            case Direction.UpRight:
                return lastHexagon.upRight;
            case Direction.DownRight:
                return lastHexagon.downRight;
            case Direction.Down:
                return lastHexagon.down;
            case Direction.DownLeft:
                return lastHexagon.downLeft;
            case Direction.UpLeft:
                return lastHexagon.upLeft;
        }

        return lastHexagon;
    }
    
    public void HideMovementPossibilities()
    {
        if (MovementPossibilities.Count == 0) return;
        foreach (var possibility in MovementPossibilities)
        {
            var hexagon = possibility.Value;
            hexagon.isMovementPossibility = false;
        }
        foreach (var possibilityPrefab in GameManager.Instance.movementPossibilityPrefabs)
        {
            Destroy(possibilityPrefab);
        }
    }
}
