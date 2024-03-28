using System;
using UnityEngine;

public class Manager_Gems : MonoBehaviour
{
    public static Action<int> OnSendGemsCount;

    [SerializeField] private string m_gemsSaveTag = "CurrentLevelIndex";

    
    private int m_gemsCount;


    private void OnEnable()
    {
        PiggyBank.OnGrantGemReward += OnGrantGemReward;
    }

    private void OnDisable()
    {
        PiggyBank.OnGrantGemReward -= OnGrantGemReward;
    }


    private void Start()
    {
        LoadGemsAmount();
    }

    private void LoadGemsAmount()
    {
        if (PlayerPrefs.HasKey(m_gemsSaveTag))
        {
            m_gemsCount = PlayerPrefs.GetInt(m_gemsSaveTag);
        }
        else
        {
            m_gemsCount = 0;
            SaveGemsAmount();
        }
        
        OnSendGemsCount?.Invoke(m_gemsCount);
    }

    private void SaveGemsAmount()
    {
        PlayerPrefs.SetInt(m_gemsSaveTag, m_gemsCount);
    }

    private void OnGrantGemReward(int gainedGems)
    {
        m_gemsCount += gainedGems;
        SaveGemsAmount();
        OnSendGemsCount?.Invoke(m_gemsCount);
    }
}
