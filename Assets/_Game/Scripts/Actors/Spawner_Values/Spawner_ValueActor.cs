using System;
using System.Collections;
using UnityEngine;

public class Spawner_ValueActor : MonoBehaviour
{
    public static Action<ValueActor_Value, Vector3, float, float> OnSpawnValue;
    public Action<float> OnSendRemainingValueToSpawn;

    [SerializeField] private int m_initialAmountToSpawn = 100;
    [SerializeField] private float m_maxEjectionStrength = 250f;
    [SerializeField] private float m_splitEjectionStrength = 50f;
    [SerializeField] private float m_initialValue = 1f;
    [SerializeField] private float m_initialSpawnSpeed = 1f;

    [Header("References")] [SerializeField]
    private ValueActor_Value m_valuePrefab = null;

    [SerializeField] private GameObject m_spawnPosition = null;
    [SerializeField] private Transform m_valuesParent = null;


    private ValueActor_Value m_valueBuffer;
    private float m_spawnTimer;
    private float m_currentSpawnSpeed;
    private float m_currentValueToSpawn;
    private int m_remainingValueToSpawn;
    private bool m_isContinuousSpawningEnabled;
    private bool m_isInCooldown;
    private bool m_isSpawningEnabled;

    private void OnEnable()
    {
        Controller.OnTapBegin += StartSpawning;
        Controller.OnRelease += StopSpawning;

        Controller_LevelSection.OnStartLoadingNextSectionLevel += DisableSpawning;
        Controller_LevelSection.OnNextLevelSectionLoaded += EnableSpawning;

        ValueActor_Value.OnHitSplitter += OnHitSplitter;
        ValueBonus.OnGrantBonus += OnGrantBonus;
    }

    private void OnDisable()
    {
        Controller.OnTapBegin -= StartSpawning;
        Controller.OnRelease -= StopSpawning;

        Controller_LevelSection.OnStartLoadingNextSectionLevel -= DisableSpawning;
        Controller_LevelSection.OnNextLevelSectionLoaded -= EnableSpawning;

        ValueActor_Value.OnHitSplitter -= OnHitSplitter;
        ValueBonus.OnGrantBonus -= OnGrantBonus;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        m_isInCooldown = false;
        
        m_currentSpawnSpeed = m_initialSpawnSpeed;
        
        m_remainingValueToSpawn = m_initialAmountToSpawn;
        OnSendRemainingValueToSpawn?.Invoke(m_remainingValueToSpawn);
        
        UpdateSpawnValue(m_initialValue);
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

        SpawnValue(m_spawnPosition.transform.position, m_spawnPosition.transform.forward, m_currentValueToSpawn, 1f);
        
        m_remainingValueToSpawn--;
        OnSendRemainingValueToSpawn?.Invoke(m_remainingValueToSpawn);
        
        m_isInCooldown = true;
        m_spawnTimer = 0f;
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
                UpdateSpawnValue(m_currentValueToSpawn + value);
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
                UpdateSpawnValue(m_currentValueToSpawn - value);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    
    
    private void UpdateSpawnValue(float newValue)
    {
        m_currentValueToSpawn = newValue;
        OnSendRemainingValueToSpawn?.Invoke(m_remainingValueToSpawn);
    }

    
    private void SpawnValue(Vector3 spawnPosition, Vector3 ejectionDirection, float value, float chargeProgression)
    {
        m_valueBuffer = Instantiate(m_valuePrefab, spawnPosition, Quaternion.identity);

        OnSpawnValue?.Invoke(m_valueBuffer, ejectionDirection, value,
            m_maxEjectionStrength * chargeProgression);
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