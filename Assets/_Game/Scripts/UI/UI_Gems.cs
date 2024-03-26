using TMPro;
using UnityEngine;

public class UI_Gems : MonoBehaviour
{
    [SerializeField] private TMP_Text m_gemsText = null;

    [SerializeField] private string m_gemCharCode = "";

    private void OnEnable()
    {
        Manager_Gems.OnSendGemsCount += OnSendGemsCount;
    }

    private void OnDisable()
    {
        Manager_Gems.OnSendGemsCount -= OnSendGemsCount;
    }


    private void OnSendGemsCount(int amount)
    {
        m_gemsText.text = m_gemCharCode + amount.ToString("F0");
    }
    
}
