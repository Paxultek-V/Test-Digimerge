using System;
using System.Collections;
using UnityEngine;

public class Spawner_Stats : MonoBehaviour
{
    public static System.Action<BonusType, float> OnSendBonusRemainingDuration;
    public static System.Action OnStopBonus;

    [SerializeField] private  Spawner_Value_SO m_spawnerData = null;
    
    public float CurrentSpawnSpeed
    {
        get => m_spawnerData.baseSpawnSpeed + m_bonusSpawnFrequency + m_upgradeSpawnSpeed;
    }

    public float CurrentValueToSpawn
    {
        get => m_spawnerData.baseValueToSpawn + m_bonusValueToSpawn + m_upgradeValueToSpawn;
    }

    private float m_bonusSpawnFrequency;
    private float m_bonusValueToSpawn;

    private float m_upgradeSpawnSpeed;
    private float m_upgradeValueToSpawn;

    
    
    private void OnEnable()
    {
        ValueBonus.OnGrantBonus += OnGrantBonus;
        Manager_GameState.OnBroadcastGameState += OnBroadcastGameState;
    }

    private void OnDisable()
    {
        ValueBonus.OnGrantBonus -= OnGrantBonus;
        Manager_GameState.OnBroadcastGameState -= OnBroadcastGameState;
    }

    private void OnBroadcastGameState(GameState state)
    {
        StopAllCoroutines();
        OnStopBonus?.Invoke();
    }
    
    private void OnGrantBonus(BonusType type, float value, float duration)
    {
        StartCoroutine(BonusCoroutine(type, value, duration));
    }


    private IEnumerator BonusCoroutine(BonusType type, float value, float duration)
    {
        switch (type)
        {
            case BonusType.SpawnFrequency:
                m_bonusSpawnFrequency += value;
                break;
            case BonusType.BoostInitialValue:
                m_bonusValueToSpawn += value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        yield return BonusDurationCoroutine(type, duration);

        switch (type)
        {
            case BonusType.SpawnFrequency:
                m_bonusSpawnFrequency -= value;
                break;
            case BonusType.BoostInitialValue:
                m_bonusValueToSpawn -= value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private IEnumerator BonusDurationCoroutine(BonusType type, float bonusDutation)
    {
        float bonusTimer = 0f;

        while (bonusTimer < bonusDutation)
        {
            bonusTimer += Time.unscaledDeltaTime;
            OnSendBonusRemainingDuration?.Invoke(type, bonusDutation - bonusTimer);
            yield return new WaitForEndOfFrame();
        }
    }
}