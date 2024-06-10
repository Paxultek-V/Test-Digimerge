using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_StartGame : MonoBehaviour, IPointerDownHandler
{
    public static Action OnStartGameButtonPressed;

    private Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        Manager_GameState.OnBroadcastGameState += OnGameStateChanged;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_button.interactable)
        {
            OnStartGameButtonPressed?.Invoke();
            Unlock();
        }
    }
    private void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                Lock();
                break;
            default:
                break;
        }
    }
    
    private void Lock()
    {
        
        m_button.enabled = true;
        ControllerManager.IsInputBlocked = true;
    }

    private void Unlock()
    {
        m_button.enabled = false;
        ControllerManager.IsInputBlocked = false;
    }

    private void OnDestroy()
    {
        Manager_GameState.OnBroadcastGameState -= OnGameStateChanged;
    }
    
    void Start()
    {
        Lock();
    }
    
}