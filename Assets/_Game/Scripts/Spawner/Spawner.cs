using System;
using System.Collections;
using TMPro;
using UnityEngine;

public enum SpawnerType
{
    ChargeButton,
    Automatic
}

public class Spawner : MonoBehaviour
{
    public static Action<ValueActor_Value, Vector3, float, float> OnSpawnValue;

    [Header("Parameters")]
    [SerializeField] private SpawnerType m_spawnerType;
    [SerializeField] private float m_maxEjectionStrength = 250f;
    [SerializeField] private float m_initialValue = 1f;
    [SerializeField] private float m_initialSpawnSpeed = 1f;

    [Header("References")]
    [SerializeField] private ValueActor_Value m_valuePrefab = null;
    [SerializeField] private GameObject m_spawnPosition = null;
    [SerializeField] private Transform m_valuesParent = null;
    [SerializeField] private TMP_Text m_initialValueText = null;


    private ValueActor_Value m_valueBuffer;
    private float m_spawnTimer;
    private float m_currentSpawnSpeed;
    private float m_currentValueToSpawn;
    private bool m_isContinuousSpawningEnabled;
    private bool m_isInCooldown;

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
    }

    private void Start()
    {
        m_isInCooldown = false;
        m_currentSpawnSpeed = m_initialSpawnSpeed;
        UpdateSpawnValue(m_initialValue);
    }

    private void Update()
    {
        if (m_isInCooldown)
        {
            m_spawnTimer += Time.deltaTime;
            if (m_spawnTimer > 1 / m_currentSpawnSpeed)
                m_isInCooldown = false;
            return;
        }

        if (m_isContinuousSpawningEnabled == false)
            return;

        SpawnValue(1f);
        m_isInCooldown = true;
        m_spawnTimer = 0f;
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

        yield return new WaitForSeconds(duration);

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
        SpawnValue(chargeProgression);
    }

    private void UpdateSpawnValue(float newValue)
    {
        m_currentValueToSpawn = newValue;
        m_initialValueText.text = "$" + m_currentValueToSpawn.ToString("F0");
    }

    private void SpawnValue(float chargeProgression)
    {
        m_valueBuffer = Instantiate(m_valuePrefab, m_spawnPosition.transform.position, Quaternion.identity,
            m_valuesParent);

        OnSpawnValue?.Invoke(m_valueBuffer, m_spawnPosition.transform.forward, m_initialValue,
            m_maxEjectionStrength * chargeProgression);
    }
}