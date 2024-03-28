using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_Stats : MonoBehaviour
{

    public float SpawningSpeed
    {
        get => CalculateSpawningSpeed();
    } 




    private float CalculateSpawningSpeed()
    {
        return 0f;
    }
    
    
    
    
    /*private IEnumerator BonusCoroutine(BonusType type, float value, float duration)
    {
        switch (type)
        {
            case BonusType.SpawnFrequency:
                m_currentSpawnSpeed += value;
                break;
            case BonusType.BoostInitialValue:
                UpdateValueToSpawn(m_currentValueToSpawn + value);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        yield return new WaitForSecondsRealtime(duration);

        switch (type)
        {
            case BonusType.SpawnFrequency:
                m_currentSpawnSpeed -= value;
                break;
            case BonusType.BoostInitialValue:
                UpdateValueToSpawn(m_currentValueToSpawn - value);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }*/

    
}
