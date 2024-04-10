using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Controller_BackgroundColor : MonoBehaviour
{
    [SerializeField] private List<Color> m_colorList = null;


    private Color m_currentColor;


    private void OnEnable()
    {
        Manager_GameState.OnBroadcastGameState += OnBroadcastGameState;
    }

    private void OnDisable()
    {
        Manager_GameState.OnBroadcastGameState -= OnBroadcastGameState;
    }


    private void OnBroadcastGameState(GameState state)
    {
        if(state == GameState.MainMenu)
            ApplyNewColor();
    }


    private void ApplyNewColor()
    {
        int randomIndex = Random.Range(0, m_colorList.Count);

        Camera.main.backgroundColor = m_colorList[randomIndex];
    }

}
