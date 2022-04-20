using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : Hexagon
{
    private static Bee _instance;
    public static Bee Instance => _instance ??= FindObjectOfType<Bee>();
    
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
    
    public void Move(Hexagon src, Hexagon dest, Sprite sprite, HexagonColor color, Type type)
    {
        src.GetComponent<SpriteRenderer>().sprite = idleSprite;
        src._color = HexagonColor.NotDefined;
        src._type = Type.Empty;

        dest.GetComponent<SpriteRenderer>().sprite = sprite;
        dest._color = color;
        dest._type = type;

        GameManager.Instance.RemoveFilledHexagon(src);
        GameManager.Instance.AddFilledHexagon(dest);
        HideMovementPossibilities();
    }

    public void ShowMovementPossibilities(Hexagon clickedHexagon)
    {
        HideMovementPossibilities();
        var emptyAdjacent = clickedHexagon.GetEmptyAdjacent();
        MovementPossibilities = new Dictionary<int, Hexagon>();
        
        foreach (var adj in emptyAdjacent)
        {
            var nonEmptyAdjs = adj.GetNonEmptyAdjacent();
            if (nonEmptyAdjs.Count == 1 && nonEmptyAdjs.Contains(clickedHexagon)) continue;
            MovementPossibilities.Add(adj.id, adj);
        }
        
        foreach (var possibility in MovementPossibilities)
        {
            var hexagon = possibility.Value;
            hexagon.isMovementPossibility = true;
            var pos = hexagon.transform.position;
            var rot = hexagon.transform.rotation;
            var possibilityObject = Instantiate(possibilityPrefab, pos, rot);
            GameManager.Instance.movementPossibilityPrefabs.Add(possibilityObject);
        }
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
