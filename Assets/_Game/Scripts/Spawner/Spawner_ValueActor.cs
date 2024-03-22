using System;
using System.Collections;
using TMPro;
using UnityEngine;

public enum SpawnerType
{
    ChargeButton,
    Automatic
}

public class Spawner_ValueActor : MonoBehaviour
{
    public static Action<ValueActor_Value, Vector3, float, float> OnSpawnValue;

    [Header("Parameters")] [SerializeField]
    private SpawnerType m_spawnerType;

    [SerializeField] private float m_maxEjectionStrength = 250f;
    [SerializeField] private float m_splitEjectionStrength = 50f;
    [SerializeField] private float m_initialValue = 1f;
    [SerializeField] private float m_initialSpawnSpeed = 1f;

    [Header("References")] [SerializeField]
    private ValueActor_Value m_valuePrefab = null;

    [SerializeField] private GameObject m_spawnPosition = null;
    [SerializeField] private Transform m_valuesParent = null;
    [SerializeField] private TMP_Text m_initialValueText = null;


    private ValueActor_Value m_valueBuffer;
    private float m_spawnTimer;
    private float m_currentSpawnSpeed;
    private float m_currentValueToSpawn;
    private bool m_isContinuousSpawningEnabled;
    private bool m_isInCooldown;
    private bool m_isSpawningEnabled;

    private void OnEnable()
    {
        switch (m_spawnerType)
        {
            case SpawnerType.ChargeButton:
                SpawnerChargingButton.OnFinishedCharging += OnFinishedCharging;
                break;
            case SpawnerType.Automatic:
                Controller.OnTapBegin += StartSpawning;
                Controller.OnRelease += StopSpawning;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        ValueBonus.OnGrantBonus += OnGrantBonus;
        Manager_Level.OnStartLoadingNextLevel += DisableSpawning;
        Manager_Level.OnNextLevelLoaded += EnableSpawning;
        ValueActor_Value.OnHitSplitter += OnHitSplitter;
    }

    private void OnDisable()
    {
        switch (m_spawnerType)
        {
            case SpawnerType.ChargeButton:
                SpawnerChargingButton.OnFinishedCharging -= OnFinishedCharging;
                break;
            case SpawnerType.Automatic:
                Controller.OnTapBegin -= StartSpawning;
                Controller.OnRelease -= StopSpawning;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        ValueBonus.OnGrantBonus -= OnGrantBonus;
        Manager_Level.OnStartLoadingNextLevel -= DisableSpawning;
        Manager_Level.OnNextLevelLoaded -= EnableSpawning;
        ValueActor_Value.OnHitSplitter -= OnHitSplitter;
    }

    private void Start()
    {
        m_isInCooldown = false;
        m_currentSpawnSpeed = m_initialSpawnSpeed;
        UpdateSpawnValue(m_initialValue);
    }

    private void Update()
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

        SpawnValue(m_spawnPosition.transform.position, m_spawnPosition.transform.forward, m_currentValueToSpawn, 1f);
        m_isInCooldown = true;
        m_spawnTimer = 0f;
    }

    private void EnableSpawning()
    {
        m_isSpawningEnabled = true;
    }

    private void DisableSpawning()
    {
        m_isSpawningEnabled = false;
    }

    private void OnHitSplitter(Vector3 splitPosition, float value)
    {
        SpawnValue(splitPosition,(Vector3.up + Vector3.left).normalized, (int)(value/2f), m_splitEjectionStrength);
        SpawnValue(splitPosition,(Vector3.up + Vector3.right).normalized, (int)(value/2f), m_splitEjectionStrength);
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

    private void StartSpawning(Vector3 cursorPosition)
    {
        m_isContinuousSpawningEnabled = true;
    }

    private void StopSpawning(Vector3 cursorPosition)
    {
        m_isContinuousSpawningEnabled = false;
    }


    private void OnFinishedCharging(float chargeProgression)
    {
        //SpawnValue(chargeProgression);
    }

    private void UpdateSpawnValue(float newValue)
    {
        m_currentValueToSpawn = newValue;
        m_initialValueText.text = "$" + m_currentValueToSpawn.ToString("F0");
    }

    private void SpawnValue(Vector3 spawnPosition, Vector3 ejectionDirection, float value, float chargeProgression)
    {
        m_valueBuffer = Instantiate(m_valuePrefab, spawnPosition, Quaternion.identity,
            m_valuesParent);

        OnSpawnValue?.Invoke(m_valueBuffer, ejectionDirection, value,
            m_maxEjectionStrength * chargeProgression);
    }
}