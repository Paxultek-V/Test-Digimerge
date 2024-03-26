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
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_button.interactable)
        {
            OnStartGameButtonPressed?.Invoke();
        }
    }
}