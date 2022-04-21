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
    public Sprite idleSprite;
    public GameObject possibilityPrefab;

    [HideInInspector] public Hexagon up;
    [HideInInspector] public Hexagon down;
    [HideInInspector] public Hexagon upRight;
    [HideInInspector] public Hexagon upLeft;
    [HideInInspector] public Hexagon downRight;
    [HideInInspector] public Hexagon downLeft;

    [HideInInspector] public bool isPossibility;
    [HideInInspector] public bool isMovementPossibility;
    
    public Stack<SavedHexagon> stack = new Stack<SavedHexagon>();

    private Hexagon[] AllHexagons => GameManager.Instance.allHexagons;
    

    private PlayerBlack PlayerBlack => GameManager.Instance.playerBlack;
    private PlayerWhite PlayerWhite => GameManager.Instance.playerWhite;

    private Dictionary<int, Hexagon> Possibilities
    {
        get
        {
            return GameManager.Instance.possibilities;
        }
        set
        {
            GameManager.Instance.possibilities = value;
        }
    }
    
    private void Update()
    {
        
    }

    public void ClickedHexagon(Hexagon clickedHexagon)
    {
        if (!clickedHexagon) return;
        if (GameManager.Instance.isBlackWon || GameManager.Instance.isWhiteWon) return;
        
        if (clickedHexagon.isPossibility)
        {
            HexagonColor color = GameManager.Instance.lastSelectedColor;
            Sprite sprite = GameManager.Instance.lastSelectedSprite;
            Set(clickedHexagon, color, sprite, GameManager.Instance.lastSelectedType);
        }
        else if (clickedHexagon.isMovementPossibility)
        {
            var hexagonToMove = GameManager.Instance.clickedToMove;
            if (hexagonToMove)
            {
                Move(hexagonToMove, clickedHexagon);
            }
            GameManager.Instance.clickedToMove = null;
        }
        else if (clickedHexagon._type != Type.Empty)
        {
            if (clickedHexagon._color == HexagonColor.Black && GameManager.Instance.turn == Turn.White ||
                clickedHexagon._color == HexagonColor.White && GameManager.Instance.turn == Turn.Black)
            {
                return;
            }
            ShowMovementPossibility(clickedHexagon);
        }
    }

    public void Move(Hexagon src, Hexagon dest)
    {
        var sprite = src.GetComponent<SpriteRenderer>().sprite;
        switch (src._type)
        {
            case Type.Ant:
                Ant.Instance.Move(src, dest, sprite, src._color, src._type);
                break;
            case Type.Bee:
                Bee.Instance.Move(src, dest, sprite, src._color, src._type);
                break;
            case Type.Beetle:
                Beetle.Instance.Move(src, dest, sprite, src._color, src._type);
                break;
            case Type.GrassHopper:
                GrassHopper.Instance.Move(src, dest, sprite, src._color, src._type);
                break;
            case Type.Spider:
                Spider.Instance.Move(src, dest, sprite, src._color, src._type);
                break;
        }

        GameManager.Instance.IsGameFinish();
        
        ChangeTurn();
    }

    public List<Hexagon> GetAdjacent()
    {
        List<Hexagon> adjacent = new List<Hexagon>();
        adjacent.Add(up);
        adjacent.Add(upRight);
        adjacent.Add(downRight);
        adjacent.Add(down);
        adjacent.Add(downLeft);
        adjacent.Add(upLeft);
        return adjacent;
    }

    public List<Hexagon> GetEmptyAdjacent()
    {
        var emptyAdjs = new List<Hexagon>();
        var adjs = GetAdjacent();
        foreach (var adj in adjs)
        {
            if (adj._type == Type.Empty)
                emptyAdjs.Add(adj);
        }

        return emptyAdjs;
    }

    public List<Hexagon> GetNonEmptyAdjacent()
    {
        var nonEmptyAdjs = new List<Hexagon>();
        var adjs = GetAdjacent();
        foreach (var adj in adjs)
        {
            if (adj._type != Type.Empty)
            {
                nonEmptyAdjs.Add(adj);
            }
        }

        return nonEmptyAdjs;
    }

    public Hexagon GetOneOfNonEmptyAdjacent()
    {
        var adjs = GetAdjacent();
        foreach (var adj in adjs)
        {
            if (adj._type != Type.Empty) return adj;
        }

        return null;
    }

    public static Hexagon GetById(int id)
    {
        return GameManager.Instance.allHexagons[id];
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
        ChangeTurn();
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

        hexagon.isPossibility = false;
        hexagon.AddToStack();

        if (hexagon._type == Type.Bee)
        {
            if (hexagon._color == HexagonColor.Black)
            {
                GameManager.Instance.blackBeeId = hexagon.id;
            }
            else
            {
                GameManager.Instance.whiteBeeId = hexagon.id;
            }
        }

        HidePossibilities();
    }

    public void ShowPossibilities()
    {
        HidePossibilities();
        var _hiveMargin = GameManager.Instance.hiveMargins;
        Possibilities = new Dictionary<int, Hexagon>();
        if (GameManager.Instance.filledHexagons.Count >= 2)
        {
            foreach (var margin in _hiveMargin)
            {
                var hexagon = margin.Value;
                if ((GameManager.Instance.turn == Turn.Black && !hexagon.HasWhiteAdjacent()) ||
                    (GameManager.Instance.turn == Turn.White && !hexagon.HasBlackAdjacent()))
                {
                    Possibilities.Add(hexagon.id, hexagon);
                }
            }
        }
        else
        {
            Possibilities = _hiveMargin;
        }
        
        foreach (var possibility in Possibilities)
        {
            var hexagon = possibility.Value;
            hexagon.isPossibility = true;
            var pos = hexagon.transform.position;
            pos.z -= 0.01f;
            var rot = hexagon.transform.rotation;
            var possibilityPrefab = Instantiate(this.possibilityPrefab, pos, rot);
            GameManager.Instance.possibilityPrefabs.Add(possibilityPrefab);
        }
    }

    public void HidePossibilities()
    {
        // if (_possibilities.Count == 0) return;
        foreach (var possibility in Possibilities)
        {
            var hexagon = possibility.Value;
            hexagon.isPossibility = false;
        }
        foreach (var possibilityPrefab in GameManager.Instance.possibilityPrefabs)
        {
            Destroy(possibilityPrefab);
        }
    }

    public void ShowMovementPossibility(Hexagon hexagon)
    {
        if (!IsBeeUsed()) return;
        if (!CanMove(hexagon)) return;
        SetSelectedHexagonValues(hexagon);
        
        switch (hexagon._type)
        {
            case Type.Ant:
                Ant.Instance.ShowMovementPossibilities(hexagon);
                return;
            case Type.Bee:
                Bee.Instance.ShowMovementPossibilities(hexagon);
                return;
            case Type.Beetle:
                Beetle.Instance.ShowMovementPossibilities(hexagon);
                return;
            case Type.GrassHopper:
                GrassHopper.Instance.ShowMovementPossibilities(hexagon);
                return;
            case Type.Spider:
                Spider.Instance.ShowMovementPossibilities(hexagon);
                return;
        }
    }

    private void SetSelectedHexagonValues(Hexagon clickedHexagon)
    {
        Sprite sprite = clickedHexagon._color == HexagonColor.Black ? blackSprite : whiteSprite;
            
        GameManager.Instance.lastSelectedType = clickedHexagon._type;
        GameManager.Instance.lastSelectedSprite = sprite;
        GameManager.Instance.lastSelectedColor = clickedHexagon._color;
        GameManager.Instance.clickedToMove = clickedHexagon;
    }
    

    private void ChangeTurn()
    {
        if (GameManager.Instance.turn == Turn.Black)
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
        if (up._color == HexagonColor.Black) return true;
        if (upRight._color == HexagonColor.Black) return true;
        if (downRight._color == HexagonColor.Black) return true;
        if (down._color == HexagonColor.Black) return true;
        if (downLeft._color == HexagonColor.Black) return true;
        if (upLeft._color == HexagonColor.Black) return true;
        return false;
    }
    
    private bool HasWhiteAdjacent()
    {
        if (up._color == HexagonColor.White) return true;
        if (upRight._color == HexagonColor.White) return true;
        if (downRight._color == HexagonColor.White) return true;
        if (down._color == HexagonColor.White) return true;
        if (downLeft._color == HexagonColor.White) return true;
        if (upLeft._color == HexagonColor.White) return true;
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

    private bool IsBeeUsed()
    {
        if (GameManager.Instance.turn == Turn.Black && PlayerBlack.IsBeeUsed()) return true;
        if (GameManager.Instance.turn == Turn.White && PlayerWhite.IsBeeUsed()) return true;
        return false;
    }

    private bool CanMove(Hexagon hexagon)
    {
        if (hexagon._type == Type.Empty) return false;
        if (hexagon.stack.Count > 1) return true;
        bool canMove;
        var type = hexagon._type;
        hexagon._type = Type.Empty;
        GameManager.Instance.filledHexagons.Remove(hexagon.id);
        canMove = IsGraphConnected(hexagon.GetOneOfNonEmptyAdjacent());
        hexagon._type = type;
        GameManager.Instance.filledHexagons.Add(hexagon.id, hexagon);

        return canMove;
    }

    private bool IsGraphConnected(Hexagon hexagon)
    {
        int count = GameManager.Instance.filledHexagons.Count;
        List<Hexagon> visited = new List<Hexagon>();
        DFS(hexagon.id, ref visited);

        if (visited.Count == count)
        {
            return true;
        }

        return false;
    }

    private void DFS(int hexagonId, ref List<Hexagon> visited)
    {
        var hexagon = GetById(hexagonId);
        visited.Add(hexagon);
        var adjs = hexagon.GetAdjacent();
        foreach (var adj in adjs)
        {
            if (adj._type != Type.Empty && !visited.Contains(adj))
            {
                DFS(adj.id, ref visited);
            }
        }
    }

    public bool IsEmpty()
    {
        return _type == Type.Empty;
    }
    public bool IsFreeze()
    {
        var top = stack.Peek();
        
        if (top.savedColor.Equals(_color) && top.savedType.Equals(_type))
        {
            return false;
        }

        return true;
    }

    public void AddToStack()
    {
        if (stack.Count > 0 && _type != Type.Beetle) return;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        SavedHexagon savedHexagon = new SavedHexagon(_type, _color, sprite);
        stack.Push(savedHexagon);
    }

    public void RemoveFromStack()
    {
        if (stack.Count <= 0) return;
        var top = stack.Peek();
        if (top.savedColor.Equals(_color) && top.savedType.Equals(_type))
        {
            stack.Pop();
        }

        if (stack.Count > 0)
        {
            _type = stack.Peek().savedType;
            _color = stack.Peek().savedColor;
            var sprite = stack.Peek().savedSprite;
            GetComponent<SpriteRenderer>().sprite = sprite;
        }
        else
        {
            _type = Type.Empty;
            _color = HexagonColor.NotDefined;
            GetComponent<SpriteRenderer>().sprite = idleSprite;
        }
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

public enum Direction
{
    Up, UpRight, DownRight, Down, DownLeft, UpLeft
}

public struct SavedHexagon
{
    public Type savedType;
    public HexagonColor savedColor;
    public Sprite savedSprite;

    public SavedHexagon(Type type, HexagonColor color, Sprite sprite)
    {
        savedType = type;
        savedColor = color;
        savedSprite = sprite;
    }
}
