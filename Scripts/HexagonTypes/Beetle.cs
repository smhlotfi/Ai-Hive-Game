using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beetle : Hexagon
{
    private static Beetle _instance;
    public static Beetle Instance => _instance ??= FindObjectOfType<Beetle>();
    
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
    
    public void Move(Hexagon src, Hexagon dest, Sprite sprite, HexagonColor color, Type type)
    {
        src.RemoveFromStack();
        // src.GetComponent<SpriteRenderer>().sprite = idleSprite;
        // src._color = HexagonColor.NotDefined;
        // src._type = Type.Empty;

        if (dest._type == Type.Empty)
        {
            GameManager.Instance.AddFilledHexagon(dest);    
        }
        
        
        dest.GetComponent<SpriteRenderer>().sprite = sprite;
        dest._color = color;
        dest._type = type;
        dest.AddToStack();

        if (src.stack.Count <= 0)
        {
            GameManager.Instance.RemoveFilledHexagon(src);    
        }
        
        
        
        HideMovementPossibilities();
    }

    public void ShowMovementPossibilities(Hexagon clickedHexagon)
    {
        HideMovementPossibilities();
        var adjs = clickedHexagon.GetAdjacent();
        MovementPossibilities = new Dictionary<int, Hexagon>();
        
        foreach (var adj in adjs)
        {
            if (clickedHexagon.stack.Count <= 1)
            {
                var nonEmptyAdjs = adj.GetNonEmptyAdjacent();
                if (nonEmptyAdjs.Count == 1 && nonEmptyAdjs.Contains(clickedHexagon)) continue;
            }
            
            MovementPossibilities.Add(adj.id, adj);
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
