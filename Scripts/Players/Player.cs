using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerColor _color;
    

    public bool IsTurn()
    {
        return _color == PlayerColor.Black && GameManager.Instance.turn == Turn.Black || 
               _color == PlayerColor.White && GameManager.Instance.turn == Turn.White;
    }
}

public enum PlayerColor
{
    Black, White
}
