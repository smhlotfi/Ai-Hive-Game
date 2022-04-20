using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Ant : Hexagon
{
    private static Ant _instance;
    public static Ant Instance => _instance ??= FindObjectOfType<Ant>();
    
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
    
    public void SelectBlackAnt()
    {
        if (GameManager.Instance.turn == Turn.White) return;
        Select(HexagonColor.Black, blackSprite, Type.Ant);
    }
    
    public void SelectWhiteAnt()
    {
        if (GameManager.Instance.turn == Turn.Black) return;
        Select(HexagonColor.White, whiteSprite, Type.Ant);
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
        var _hiveMargin = GameManager.Instance.hiveMargins;
        MovementPossibilities = new Dictionary<int, Hexagon>();
        
        foreach (var margin in _hiveMargin)
        {
            var marginNonEmptyAdjs = margin.Value.GetNonEmptyAdjacent();
            if (marginNonEmptyAdjs.Count == 1 && marginNonEmptyAdjs.Contains(clickedHexagon)) continue;
            MovementPossibilities.Add(margin.Key, margin.Value);
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
