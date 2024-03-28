using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Spawner_ValueActor : MonoBehaviour
{
    public static Action<ValueActor_Value, Vector3, float, float> OnSpawnValue;
    public static Action<float> OnSendAdditionalTimeBeforeSpawnStopsTimer;
    public static Action OnAllValuesUsed;
    public Action<float> OnSendRemainingValueToSpawn;

    [SerializeField] private float m_initialAmountToSpawn = 100;
    [SerializeField] private float m_maxEjectionStrength = 250f;
    [SerializeField] private float m_splitEjectionStrength = 50f;
    [SerializeField] private float m_initialValue = 1f;
    [SerializeField] private float m_initialSpawnSpeed = 1f;
    [FormerlySerializedAs("m_additionalTimeBeforeSpawnStopsTime")] [SerializeField] private float m_additionalTimeBeforeSpawnStops = 3f;

    [Header("References")] [SerializeField]
    private ValueActor_Value m_valuePrefab = null;

    [SerializeField] private GameObject m_spawnPosition = null;
    [SerializeField] private Transform m_valuesParent = null;

    private List<ValueActor_Value> m_valueActorList = new List<ValueActor_Value>();
    private ValueActor_Value m_valueBuffer;
    private float m_spawnTimer;
    private float m_currentSpawnSpeed;
    private float m_currentValueToSpawn;
    private float m_remainingValueToSpawn;
    private float m_additionalTimeBeforeSpawnStopsTimer;
    
    private bool m_isSpawningEnabled; //flag to manage the overall state of spawning
    private bool m_isContinuousSpawningEnabled; //flag to manage spawning  with the input controls
    private bool m_isInCooldown; //flag to manage the spawning speed
    private bool m_canTrackRemainingValues;

    private void OnEnable()
    {
        Controller.OnTapBegin += StartSpawning;
        Controller.OnRelease += StopSpawning;

        Controller_LevelSection.OnNextLevelSectionLoaded += OnNextLevelSectionLoaded;

        ValueActor_Value.OnHitSplitter += OnHitSplitter;
        ValueBonus.OnGrantBonus += OnGrantBonus;

        PiggyBank.OnPiggyBankFinishedCollectingMoney += OnPiggyBankFinishedCollectingMoney;
        PiggyBank.OnTargetAmountReached += OnTargetAmountReached;

        ValueActor_Value.OnValueKilled += OnValueKilled;
    }

    private void OnDisable()
    {
        Controller.OnTapBegin -= StartSpawning;
        Controller.OnRelease -= StopSpawning;

        Controller_LevelSection.OnNextLevelSectionLoaded -= OnNextLevelSectionLoaded;

        ValueActor_Value.OnHitSplitter -= OnHitSplitter;
        ValueBonus.OnGrantBonus -= OnGrantBonus;

        PiggyBank.OnPiggyBankFinishedCollectingMoney -= OnPiggyBankFinishedCollectingMoney;
        PiggyBank.OnTargetAmountReached -= OnTargetAmountReached;

        ValueActor_Value.OnValueKilled -= OnValueKilled;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        m_isInCooldown = true;

        m_currentSpawnSpeed = m_initialSpawnSpeed;

        m_remainingValueToSpawn = m_initialAmountToSpawn;
        OnSendRemainingValueToSpawn?.Invoke(m_remainingValueToSpawn);

        UpdateValueToSpawn(m_initialValue);
    }

    private void Update()
    {
        ManageSpawning();
    }

    private void ManageSpawning()
    {
        if (m_isSpawningEnabled == false)
            return;

        if (m_isInCooldown)
        {
            m_spawnTimer += Time.deltaTime;
            if (m_spawnTimer > 1 / m_currentSpawnSpeed)
                m_isInCooldown = false;
            return;
        }

        if (m_isContinuousSpawningEnabled == false)
            return;

        if (m_remainingValueToSpawn <= 0)
            return;

        SpawnValue(m_spawnPosition.transform.position, m_spawnPosition.transform.forward, m_currentValueToSpawn,
            1f);
        m_remainingValueToSpawn -= m_currentValueToSpawn;

        OnSendRemainingValueToSpawn?.Invoke(m_remainingValueToSpawn);

        if (m_remainingValueToSpawn <= 0 && m_canTrackRemainingValues == false)
            m_canTrackRemainingValues = true;
        
        m_isInCooldown = true;
        m_spawnTimer = 0f;
    }

    private void OnPiggyBankFinishedCollectingMoney(float amountCollected, bool isLastPiggyBankOfLevel)
    {
        if (isLastPiggyBankOfLevel)
            return;

        m_remainingValueToSpawn = amountCollected;
        OnSendRemainingValueToSpawn?.Invoke(m_remainingValueToSpawn);
    }

    private void OnTargetAmountReached()
    {
        StartCoroutine(AdditionalTimeBeforeSpawnStops());
    }

    private IEnumerator AdditionalTimeBeforeSpawnStops()
    {
        m_additionalTimeBeforeSpawnStopsTimer = 0f;

        while (m_additionalTimeBeforeSpawnStopsTimer < m_additionalTimeBeforeSpawnStops)
        {
            m_additionalTimeBeforeSpawnStopsTimer += Time.unscaledDeltaTime;
            
            OnSendAdditionalTimeBeforeSpawnStopsTimer?.Invoke(m_additionalTimeBeforeSpawnStops - m_additionalTimeBeforeSpawnStopsTimer);
            
            yield return new WaitForEndOfFrame();
        }
        
        DisableSpawning();

        m_remainingValueToSpawn = 0;
        m_canTrackRemainingValues = true;
        
        if (m_canTrackRemainingValues && m_valueActorList.Count == 0)
        {
            OnAllValuesUsed?.Invoke();
        }
        
        OnSendRemainingValueToSpawn?.Invoke(m_remainingValueToSpawn);
    }
    
    private void OnHitSplitter(Vector3 splitPosition, float value)
    {
        SpawnValue(splitPosition, (Vector3.up + Vector3.left).normalized, (int)(value / 2f), m_splitEjectionStrength);
        SpawnValue(splitPosition, (Vector3.up + Vector3.right).normalized, (int)(value / 2f), m_splitEjectionStrength);
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
    }


    private void UpdateValueToSpawn(float newValue)
    {
        m_currentValueToSpawn = newValue;
    }


    private void SpawnValue(Vector3 spawnPosition, Vector3 ejectionDirection, float value, float chargeProgression)
    {
        m_valueBuffer = Instantiate(m_valuePrefab, spawnPosition, Quaternion.identity, m_valuesParent);

        m_valueActorList.Add(m_valueBuffer);

        OnSpawnValue?.Invoke(m_valueBuffer, ejectionDirection, value,
            m_maxEjectionStrength * chargeProgression);
    }

    private void OnValueKilled(ValueActor_Value value)
    {
        if (m_valueActorList.Remove(value))
        {
            if (m_canTrackRemainingValues && m_valueActorList.Count == 0)
            {
                OnAllValuesUsed?.Invoke();
            }
        }
    }


    private void OnNextLevelSectionLoaded()
    {
        EnableSpawning();
        m_canTrackRemainingValues = false;
    }

    private void EnableSpawning()
    {
        m_isSpawningEnabled = true;
    }


    private void DisableSpawning()
    {
        m_isSpawningEnabled = false;
    }

    private void StartSpawning(Vector3 cursorPosition)
    {
        m_isContinuousSpawningEnabled = true;
    }

    private void StopSpawning(Vector3 cursorPosition)
    {
        m_isContinuousSpawningEnabled = false;
    }
}