using System;
using UnityEngine;
using UnityEngine.UI;

public enum LoadLevelType
{
    Reload,
    LoadNext,
    LoadPrevious
}

public class Button_LoadLevel : MonoBehaviour
{
    public static Action<LoadLevelType> OnLoadLevel_ButtonPressed;

    [SerializeField] private LoadLevelType m_loadLevelType;


    private Button m_button;


    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(LoadLevelButton);
    }

    private void LoadLevelButton()
    {
        OnLoadLevel_ButtonPressed?.Invoke(m_loadLevelType);
    }
    
}
