using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [HideInInspector] public int id;
    [HideInInspector] public HexagonColor _color = HexagonColor.NotDefined;
    [HideInInspector] public Type _type;
    
    public Sprite whiteSprite;
    public Sprite blackSprite;
    [SerializeField] private GameObject possibilityPrefab;

    [HideInInspector] public Hexagon up;
    [HideInInspector] public Hexagon down;
    [HideInInspector] public Hexagon upRight;
    [HideInInspector] public Hexagon upLeft;
    [HideInInspector] public Hexagon downRight;
    [HideInInspector] public Hexagon downLeft;

    [HideInInspector] public bool isPossibility;

    private bool _isFreeze; // Beetle on that
    private List<GameObject> possibilityPrefabs = new List<GameObject>();

    private Hexagon[] AllHexagons => GameManager.Instance.allHexagons;
    private Dictionary<int, Hexagon> _possibilities = new Dictionary<int, Hexagon>();

    private PlayerBlack PlayerBlack => GameManager.Instance.playerBlack;
    private PlayerWhite PlayerWhite => GameManager.Instance.playerWhite;
    
    private void Update()
    {
        var clickedHexagon = GameManager.Instance.clickedHexagon;
        if (!clickedHexagon) return;
        if (clickedHexagon.isPossibility)
        {
            HexagonColor color = GameManager.Instance.lastSelectedColor;
            Sprite sprite = GameManager.Instance.lastSelectedSprite;
            Set(clickedHexagon, color, sprite, GameManager.Instance.lastSelectedType);
        }
    }
    
    public void Select(HexagonColor color, Sprite sprite, Type type)
    {
        if (MustBeeEnter() && type != Type.Bee) return;
        GameManager.Instance.lastSelectedType = type;
        GameManager.Instance.lastSelectedColor = color;
        GameManager.Instance.lastSelectedSprite = sprite;
        if (GameManager.Instance.IsFirstMove())
        {
            Set(AllHexagons[0], color, sprite, type);
        }
        else
        {
            if (color == HexagonColor.Black && PlayerBlack.IsZeroCount(type) || color == HexagonColor.White && PlayerWhite.IsZeroCount(type))
            {
                return;
            }
            ShowPossibilities();
        }
    }

    public void Set(Hexagon hexagon, HexagonColor color, Sprite sprite, Type type)
    {
        hexagon._type = type;
        hexagon._color = color;
        ChangeTurn(color);
        hexagon.GetComponent<SpriteRenderer>().sprite = sprite;
        if (!GameManager.Instance.filledHexagons.ContainsKey(hexagon.id))
        {
            AddMoveNumber(color);
            GameManager.Instance.AddFilledHexagon(hexagon);
            if (color == HexagonColor.Black)
            {
                PlayerBlack.DecreaseCount(type);
            }
            else
            {
                PlayerWhite.DecreaseCount(type);
            }
        }

        
        
        HidePossibilities();
    }

    public void ShowPossibilities()
    {
        var _hiveMargin = GameManager.Instance.hiveMargins;
        _possibilities = new Dictionary<int, Hexagon>();
        if (GameManager.Instance.filledHexagons.Count >= 2)
        {
            foreach (var margin in _hiveMargin)
            {
                var hexagon = margin.Value;
                if ((GameManager.Instance.turn == Turn.Black && !hexagon.HasWhiteAdjacent()) ||
                    (GameManager.Instance.turn == Turn.White && !hexagon.HasBlackAdjacent()))
                {
                    _possibilities.Add(hexagon.id, hexagon);
                }
            }
        }
        else
        {
            _possibilities = _hiveMargin;
        }
        
        foreach (var possibility in _possibilities)
        {
            var hexagon = possibility.Value;
            hexagon.isPossibility = true;
            var pos = hexagon.transform.position;
            var rot = hexagon.transform.rotation;
            var possibilityPrefab = Instantiate(this.possibilityPrefab, pos, rot);
            possibilityPrefabs.Add(possibilityPrefab);
        }
    }

    public void HidePossibilities()
    {
        if (_possibilities.Count == 0) return;
        foreach (var possibility in _possibilities)
        {
            var hexagon = possibility.Value;
            hexagon.isPossibility = false;
        }
        foreach (var possibilityPrefab in possibilityPrefabs)
        {
            Destroy(possibilityPrefab);
        }
    }
    

    private void ChangeTurn(HexagonColor color)
    {
        if (color == HexagonColor.Black)
        {
            GameManager.Instance.turn = Turn.White;
        }
        else
        {
            GameManager.Instance.turn = Turn.Black;
        }
    }

    private bool HasBlackAdjacent()
    {
        if (this.up._color == HexagonColor.Black) return true;
        if (this.upRight._color == HexagonColor.Black) return true;
        if (this.downRight._color == HexagonColor.Black) return true;
        if (this.down._color == HexagonColor.Black) return true;
        if (this.downLeft._color == HexagonColor.Black) return true;
        if (this.upLeft._color == HexagonColor.Black) return true;
        return false;
    }
    
    private bool HasWhiteAdjacent()
    {
        if (this.up._color == HexagonColor.White) return true;
        if (this.upRight._color == HexagonColor.White) return true;
        if (this.downRight._color == HexagonColor.White) return true;
        if (this.down._color == HexagonColor.White) return true;
        if (this.downLeft._color == HexagonColor.White) return true;
        if (this.upLeft._color == HexagonColor.White) return true;
        return false;
    }

    private void AddMoveNumber(HexagonColor color)
    {
        if (color == HexagonColor.Black)
        {
            PlayerBlack.moveNumber++;
        }
        else
        {
            PlayerWhite.moveNumber++;
        }
    }
    private bool MustBeeEnter()
    {
        if (GameManager.Instance.turn == Turn.Black)
        {
            return !PlayerBlack.IsBeeUsed() && PlayerBlack.moveNumber == 4;
        }
        return !PlayerWhite.IsBeeUsed() && PlayerWhite.moveNumber == 4;
    }
    

    public bool IsEmpty()
    {
        return _type == Type.Empty;
    }
    

}

public enum HexagonColor
{
    NotDefined, Black, White
}
public enum Type
{
    Empty, Bee, Beetle, GrassHopper, Ant, Spider
}

public enum HexagonDirection
{
    Up, UpRight, DownRight, Down, DownLeft, UpLeft
}
