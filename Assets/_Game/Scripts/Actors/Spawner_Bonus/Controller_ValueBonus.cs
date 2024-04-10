using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ValueBonusInfo
{
    public ValueBonus valueBonus;
    public float spawnDelay;
}

public class Controller_ValueBonus : MonoBehaviour
{
    [SerializeField] private List<ValueBonusInfo> m_valueBonusList = null;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < m_valueBonusList.Count; i++)
        {
            if (m_valueBonusList[i] != null)
            {
                m_valueBonusList[i].valueBonus.gameObject.SetActive(false);
                StartCoroutine(DelayedActivationCoroutine(m_valueBonusList[i].valueBonus.gameObject,
                    m_valueBonusList[i].spawnDelay));
            }
        }
    }


    private IEnumerator DelayedActivationCoroutine(GameObject valueBonus, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        valueBonus.SetActive(true);
    }
}