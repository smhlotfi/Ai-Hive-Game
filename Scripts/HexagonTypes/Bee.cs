using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : Hexagon
{
    [SerializeField] private Sprite whiteBeeSprite;
    [SerializeField] private Sprite blackBeeSprite;
    [SerializeField] private GameObject possibilityPrefab;
    
    private int _blackCount = 1;
    private int _whiteCount = 1;
    
    private Hexagon[] AllHexagons => GameManager.Instance.allHexagons;
    private Dictionary<int, Hexagon> _possibilities = new Dictionary<int, Hexagon>();
    
    private List<GameObject> possibilityPrefabs = new List<GameObject>();
    
    private void Update()
    {
        var clickedHexagon = GameManager.Instance.clickedHexagon;
        if (!clickedHexagon) return;
        if (clickedHexagon.isPossibility)
        {
            HexagonColor color = GameManager.Instance.turn == Turn.Black ? HexagonColor.Black : HexagonColor.White;
            Sprite sprite = GameManager.Instance.turn == Turn.Black ? blackBeeSprite : whiteBeeSprite;
            SetBee(clickedHexagon, color, sprite);
        }
    }
    
    public void SelectBlackBee()
    {
        if (GameManager.Instance.turn == Turn.White) return;
        Select(HexagonColor.Black, blackBeeSprite);
    }
    
    public void SelectWhiteBee()
    {
        if (GameManager.Instance.turn == Turn.Black) return;
        Select(HexagonColor.White, whiteBeeSprite);
    }
    
    public void Select(HexagonColor color, Sprite sprite)
    {
        GameManager.Instance.lastSelectedType = Type.Bee;
        if (GameManager.Instance.IsFirstMove())
        {
            SetBee(AllHexagons[0], color, sprite);
        }
        else
        {
            if (color == HexagonColor.Black && _blackCount == 0 || color == HexagonColor.White && _whiteCount == 0)
            {
                return;
            }
            ShowPossibilities();
        }
    }
    
    public void SetBee(Hexagon hexagon, HexagonColor color, Sprite sprite)
    {
        hexagon._type = Type.Bee;
        hexagon._color = color;
        ChangeTurn(color);
        hexagon.GetComponent<SpriteRenderer>().sprite = sprite;
        GameManager.Instance.AddFilledHexagon(hexagon);
        if (color == HexagonColor.Black)
        {
            _blackCount--;
        }
        else
        {
            _whiteCount--;
        }
        HidePossibilities();
    }
    
    public void ShowPossibilities()
    {
        _possibilities = GameManager.Instance.hiveMargins;
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
}
