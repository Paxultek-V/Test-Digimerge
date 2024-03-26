using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Manager_UI : SerializedMonoBehaviour
{
    [SerializeField] private Dictionary<GameState, CanvasGroup> m_canvasGroupDictionary = null;


    private void OnEnable()
    {
        Manager_GameState.OnBroadcastGameState += OnBroadcastGameState;
    }

    private void OnDisable()
    {
        Manager_GameState.OnBroadcastGameState -= OnBroadcastGameState;
    }

    private void OnBroadcastGameState(GameState gameState)
    {
        foreach (var canvasGroup in m_canvasGroupDictionary)
        {
            canvasGroup.Value.alpha = canvasGroup.Key == gameState ? 1 : 0;
            canvasGroup.Value.interactable = canvasGroup.Key == gameState ? true : false;
            canvasGroup.Value.blocksRaycasts = canvasGroup.Key == gameState ? true : false;
        }
    }
}