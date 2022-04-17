using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Turn turn;
    private static GameManager _instance;

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

    private void Start()
    {
        turn = Turn.White;
    }
}

public enum Turn
{
    Black, White
}
