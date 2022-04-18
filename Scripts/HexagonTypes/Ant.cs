using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Ant : MonoBehaviour
{
    [SerializeField] private Sprite whiteAntSprite;
    [SerializeField] private Sprite blackAntSprite;
    [SerializeField] private GameObject possibilityPrefab;
    
    private int _blackCount = 3;
    private int _whiteCount = 3;
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
             Sprite sprite = GameManager.Instance.turn == Turn.Black ? blackAntSprite : whiteAntSprite;
            SetAnt(clickedHexagon, color, sprite);
        }
    }

    public void SelectBlackAnt()
    {
        if (GameManager.Instance.turn == Turn.White) return;
        Select(HexagonColor.Black, blackAntSprite);
    }
    
    public void SelectWhiteAnt()
    {
        if (GameManager.Instance.turn == Turn.Black) return;
        Select(HexagonColor.White, whiteAntSprite);
    }
    public void Select(HexagonColor color, Sprite antSprite)
    {
        GameManager.Instance.lastSelectedType = Type.Ant;
        if (GameManager.Instance.IsFirstMove())
        {
            SetAnt(AllHexagons[0], color, antSprite);
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

    public void SetAnt(Hexagon hexagon, HexagonColor color, Sprite antSprite)
    {
        hexagon._type = Type.Ant;
        hexagon._color = color;
        ChangeTurn(color);
        hexagon.GetComponent<SpriteRenderer>().sprite = antSprite;
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
