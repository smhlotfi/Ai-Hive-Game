using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerBlack playerBlack;
    public PlayerWhite playerWhite;
    [HideInInspector] public Turn turn;
    
    [HideInInspector] public Type lastSelectedType = Type.Empty;
    [HideInInspector] public HexagonColor lastSelectedColor;
    [HideInInspector] public Sprite lastSelectedSprite;
    
    [HideInInspector] public Hexagon clickedHexagon;
    
    [HideInInspector] public Dictionary<int, Hexagon> hiveMargins;
    [HideInInspector] public Dictionary<int, Hexagon> filledHexagons;
    [HideInInspector] public Hexagon[] allHexagons;
    
    private static GameManager _instance;
    private int _id = 0;

    public static GameManager Instance
    {
        get
        {
            if (_instance)
                return _instance;
            _instance = FindObjectOfType<GameManager>();
            DontDestroyOnLoad(_instance);
            return _instance;
        }
    }

    private void Awake()
    {
        allHexagons = new Hexagon[BoardInitializer.Instance.GetAllHexagonsCount()];
        filledHexagons = new Dictionary<int, Hexagon>();
        hiveMargins = new Dictionary<int, Hexagon>();
    }

    private void Start()
    {
        turn = Turn.White;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickedHexagon = GetClickedHexagon();
        }
        else
        {
            clickedHexagon = null;
        }
    }

    private Hexagon GetClickedHexagon()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider != null)
        {
            return hit.collider.gameObject.GetComponent<Hexagon>();
        }

        return null;
    }


    public void AddFilledHexagon(Hexagon hexagon)
    {
        if (filledHexagons.ContainsKey(hexagon.id)) return;
        filledHexagons.Add(hexagon.id, hexagon);
        AddHiveMarginsFor(hexagon);
        RemoveHiveMarginsFor(hexagon);
    }

    public void RemoveFilledHexagon(Hexagon hexagon)
    {
        
    }

    public void AddHiveMarginsFor(Hexagon hexagon)
    {
        if (hexagon.up._type == Type.Empty && !hiveMargins.ContainsKey(hexagon.up.id))
        {
            hiveMargins.Add(hexagon.up.id, hexagon.up);
        }
        if (hexagon.upRight._type == Type.Empty && !hiveMargins.ContainsKey(hexagon.upRight.id))
        {
            hiveMargins.Add(hexagon.upRight.id, hexagon.upRight);
        }
        if (hexagon.downRight._type == Type.Empty && !hiveMargins.ContainsKey(hexagon.downRight.id))
        {
            hiveMargins.Add(hexagon.downRight.id, hexagon.downRight);
        }
        if (hexagon.down._type == Type.Empty && !hiveMargins.ContainsKey(hexagon.down.id))
        {
            hiveMargins.Add(hexagon.down.id, hexagon.down);
        }
        if (hexagon.downLeft._type == Type.Empty && !hiveMargins.ContainsKey(hexagon.downLeft.id))
        {
            hiveMargins.Add(hexagon.downLeft.id, hexagon.downLeft);
        }
        if (hexagon.upLeft._type == Type.Empty && !hiveMargins.ContainsKey(hexagon.upLeft.id))
        {
            hiveMargins.Add(hexagon.upLeft.id, hexagon.upLeft);
        }
    }

    public void RemoveHiveMarginsFor(Hexagon hexagon)
    {
        if (hiveMargins.ContainsKey(hexagon.id))
        {
            hiveMargins.Remove(hexagon.id);
        }
    }

    public bool IsFirstMove()
    {
        return filledHexagons.Count == 0;
    }

    public int GetId()
    {
        return _id++;
    }
}

public enum Turn
{
    Black, White
}
