using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Hexagon
{
    private static Spider _instance;
    private List<int> _visitedHexagons = new List<int>();
    public static Spider Instance => _instance ??= FindObjectOfType<Spider>();
    
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
        _visitedHexagons = new List<int>();
        MovementPossibilities = new Dictionary<int, Hexagon>();
        
        var firstStepHexagons = GetAdjacentInMargin(clickedHexagon);
        var secondStepHexagons = new List<Hexagon>();
        foreach (var hexagon in firstStepHexagons)
        {
            var list = GetAdjacentInMargin(hexagon);
            foreach (var item in list)
            {
                secondStepHexagons.Add(item);    
            }
        }
        foreach (var hexagon in secondStepHexagons)
        {
            var list = GetAdjacentInMargin(hexagon);
            foreach (var item in list)
            {
                MovementPossibilities.Add(item.id, item);    
            }
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

    private List<Hexagon> GetAdjacentInMargin(Hexagon hexagon)
    {
        var adjsInMargin = new List<Hexagon>();
        var emptyAdjs = hexagon.GetEmptyAdjacent();
        var hiveMargin = GameManager.Instance.hiveMargins;
        foreach (var emptyAdj in emptyAdjs)
        {
            var nonEmptyAdjs = emptyAdj.GetNonEmptyAdjacent();
            if (nonEmptyAdjs.Count == 1 && nonEmptyAdjs.Contains(emptyAdj)) continue;
            if (hiveMargin.ContainsKey(emptyAdj.id) && !_visitedHexagons.Contains(emptyAdj.id))
            {
                adjsInMargin.Add(emptyAdj);
                _visitedHexagons.Add(emptyAdj.id);
            }
        }

        return adjsInMargin;
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
