using TMPro;
using UnityEngine;

public class Text_LevelInfo : MonoBehaviour
{
    private TMP_Text m_text;


    private void OnEnable()
    {
        Manager_Level.OnLoadLevel += OnLoadLevel;
    }

    private void OnDisable()
    {
        Manager_Level.OnLoadLevel -= OnLoadLevel;
    }

    private void Awake()
    {
        m_text = GetComponent<TMP_Text>();
    }

    private void OnLoadLevel(int levelIndex)
    {
        m_text.text = "Level " + (levelIndex + 1).ToString();
    } 
    
    
}
