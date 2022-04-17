using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BoardInitializer : MonoBehaviour
{
    [SerializeField] private GameObject hexagon;
    [SerializeField] private int depth;

    private GameObject _hexagonsParent;
    private GameObject HexagonsParent
    {
        get
        {
            if (_hexagonsParent)
                return _hexagonsParent;
            _hexagonsParent = new GameObject("hexagons");
            return _hexagonsParent;
        }
    }

    private void Start()
    {
        CreateBoard();
    }

    private void CreateBoard()
    {
        var margins = new Hexagon[1];
        var centerHexagon = Instantiate(hexagon, transform.position, Quaternion.identity, HexagonsParent.transform);
        margins[0] = centerHexagon.GetComponent<Hexagon>();
        for (int i = 1; i < depth; i++)
        {
            margins = CreateNeighbours(margins);
        }
    }

    private Hexagon[] CreateNeighbours(Hexagon[] margins)
    {
        var len = (margins.Length / 6 + 1) * 6; 
        var newMargins = new Hexagon[len];
        var i = 0;
        foreach (var margin in margins)
        {
            var hexagon = margin.GetComponent<Hexagon>();
            var marginPos = margin.transform.position;
            
            SetHexagonUpRefs(hexagon, ref newMargins, ref i, marginPos);
            SetHexagonUpRightRefs(hexagon, ref newMargins, ref i, marginPos);
            SetHexagonDownRightRefs(hexagon, ref newMargins, ref i, marginPos);
            SetHexagonDownRefs(hexagon, ref newMargins, ref i, marginPos);
            SetHexagonDownLeftRefs(hexagon, ref newMargins, ref i, marginPos);
            SetHexagonUpLeftRefs(hexagon, ref newMargins, ref i, marginPos);
        }

        return newMargins;
    }

    private void SetHexagonUpRefs(Hexagon hexagon, ref Hexagon[] newMargins, ref int i, Vector2 marginPos)
    {
        Hexagon upHexagon;
        if (!hexagon.up)
        {
            var pos = new Vector2(marginPos.x, marginPos.y + 1.4f);
            upHexagon = CreateHexagon(pos);
            newMargins[i++] = upHexagon;
        }
        else
        {
            upHexagon = hexagon.up;
        }
        
        hexagon.up = upHexagon;
        upHexagon.down = hexagon;
    }

    private void SetHexagonUpRightRefs(Hexagon hexagon, ref Hexagon[] newMargins, ref int i, Vector2 marginPos)
    {
        Hexagon upRightHexagon;
        if (!hexagon.upRight)
        {
            var pos = new Vector2(marginPos.x + 1.2f, marginPos.y + .7f);
            upRightHexagon = CreateHexagon(pos);
            newMargins[i++] = upRightHexagon;
        }
        else
        {
            upRightHexagon = hexagon.upRight;
        }
        
        hexagon.upRight = upRightHexagon;
        hexagon.up.downRight = upRightHexagon;
        upRightHexagon.downLeft = hexagon;
        upRightHexagon.upLeft = hexagon.up;
        
    }

    private void SetHexagonDownRightRefs(Hexagon hexagon, ref Hexagon[] newMargins, ref int i, Vector2 marginPos)
    {
        Hexagon downRightHexagon;
        if (!hexagon.downRight)
        {
            var pos = new Vector2(marginPos.x + 1.2f, marginPos.y - .7f);
            downRightHexagon = CreateHexagon(pos);
            newMargins[i++] = downRightHexagon;
        }
        else
        {
            downRightHexagon = hexagon.downRight;
        }
        
        hexagon.downRight = downRightHexagon;
        hexagon.upRight.down = downRightHexagon;
        downRightHexagon.up = hexagon.upRight;
        downRightHexagon.upLeft = hexagon;
    }
    private void SetHexagonDownRefs(Hexagon hexagon, ref Hexagon[] newMargins, ref int i, Vector2 marginPos)
    {
        Hexagon downHexagon;
        if (!hexagon.down)
        {
            var pos = new Vector2(marginPos.x, marginPos.y - 1.4f);
            downHexagon = CreateHexagon(pos);
            newMargins[i++] = downHexagon;
        }
        else
        {
            downHexagon = hexagon.down;
        }
        hexagon.down = downHexagon;
        hexagon.downRight.downLeft = downHexagon;
        downHexagon.up = hexagon;
        downHexagon.upRight = hexagon.downRight;
        
    }
    private void SetHexagonDownLeftRefs(Hexagon hexagon, ref Hexagon[] newMargins, ref int i, Vector2 marginPos)
    {
        Hexagon downLeftHexagon;
        if (!hexagon.downLeft)
        {
            var pos = new Vector2(marginPos.x - 1.2f, marginPos.y - .7f);
            downLeftHexagon = CreateHexagon(pos);
            newMargins[i++] = downLeftHexagon;
        }
        else
        {
            downLeftHexagon = hexagon.downLeft;
        }
        
        hexagon.downLeft = downLeftHexagon;
        hexagon.down.upLeft = downLeftHexagon;
        downLeftHexagon.upRight = hexagon;
        downLeftHexagon.downRight = hexagon.down;
    }
    private void SetHexagonUpLeftRefs(Hexagon hexagon, ref Hexagon[] newMargins, ref int i, Vector2 marginPos)
    {
        Hexagon upLeftHexagon;
        if (!hexagon.upLeft)
        {
            var pos = new Vector2(marginPos.x - 1.2f, marginPos.y + .7f);
            upLeftHexagon = CreateHexagon(pos);
            newMargins[i++] = upLeftHexagon;
        }
        else
        {
            upLeftHexagon = hexagon.upLeft;
        }
        
        hexagon.upLeft = upLeftHexagon;
        hexagon.downLeft.up = upLeftHexagon;
        hexagon.up.downLeft = upLeftHexagon;
        upLeftHexagon.upRight = hexagon.up;
        upLeftHexagon.downRight = hexagon;
        upLeftHexagon.down = hexagon.downLeft;
    }

    private Hexagon CreateHexagon(Vector2 pos)
    {
        var hexagonGameObject = Instantiate(hexagon, pos, Quaternion.identity, HexagonsParent.transform);
        return hexagonGameObject.GetComponent<Hexagon>();
    }
}
