using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Spawner_AmountToSpawn_UI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_valueToSpawnText = null;

    [SerializeField] private Spawner_ValueActor m_spawnerValueActor = null;
    
    private void OnEnable()
    {
        m_spawnerValueActor.OnSendRemainingValueToSpawn += UpdateValueToSpawn;
    }

    private void OnDisable()
    {
        m_spawnerValueActor.OnSendRemainingValueToSpawn -= UpdateValueToSpawn;
    }

    private void UpdateValueToSpawn(float newValue)
    {
        m_valueToSpawnText.text = "$" + newValue.ToString("F0");
    }
}
