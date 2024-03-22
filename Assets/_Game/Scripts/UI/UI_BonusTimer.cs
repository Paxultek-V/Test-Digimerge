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
        ValueBonus.OnGrantBonus += OnGrantBonus;
    }

    private void OnDisable()
    {
        ValueBonus.OnGrantBonus -= OnGrantBonus;
    }

    private void Start()
    {
        m_text.gameObject.SetActive(false);
    }

    private void OnGrantBonus(BonusType type, float value, float duration)
    {
        if (m_typeToTrack != type)
            return;

        StartCoroutine(TimerCoroutine(type, value, duration));
    }


    private IEnumerator TimerCoroutine(BonusType type, float value, float duration)
    {
        m_text.gameObject.SetActive(true);

        string prefix = type == BonusType.SpawnFrequency ? spawnRatePrefix : moneyBoostPrefix;
        float timer = 0f;
        int minutes = 0;
        string minutesString = "";
        int seconds = 0;
        string secondsString = "";

        while (timer < duration)
        {
            minutes = (int)(duration - timer) / 60;
            seconds = (int)(duration - timer) % 60;

            timer += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();

            minutesString = minutes < 10 ? "0" : "" + minutes.ToString();
            secondsString = seconds < 10 ? "0" : "" + seconds.ToString();

            m_text.text = prefix + minutesString + ":" + secondsString;
        }


        m_text.gameObject.SetActive(false);
    }
}