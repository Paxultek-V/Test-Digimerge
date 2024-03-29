using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_BonusTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text m_text = null;

    [SerializeField] private BonusType m_typeToTrack;

    [SerializeField] private string moneyBoostPrefix = "Money boost\n";
    [SerializeField] private string spawnRatePrefix = "Spawn boost\n";

    private void OnEnable()
    {
        Spawner_Stats.OnSendBonusRemainingDuration += OnSendBonusRemainingDuration;
    }

    private void OnDisable()
    {
        Spawner_Stats.OnSendBonusRemainingDuration -= OnSendBonusRemainingDuration;
    }

    private void Start()
    {
        m_text.gameObject.SetActive(false);
    }

    private void OnSendBonusRemainingDuration(BonusType type, float duration)
    {
        if (m_typeToTrack != type)
            return;

        if (duration <= 0)
        {
            m_text.gameObject.SetActive(false);
            return;
        }

        m_text.gameObject.SetActive(true);
        
        string prefix = type == BonusType.SpawnFrequency ? spawnRatePrefix : moneyBoostPrefix;
        
        float minutes = duration / 60;
        string minutesString = (minutes < 10 ? "0" : "") + minutes.ToString("F0");
        float seconds = duration % 60;
        string secondsString = (seconds < 10 ? "0" : "") + seconds.ToString("F0");
        
        m_text.text = prefix + minutesString + ":" + secondsString;
    }
    
}