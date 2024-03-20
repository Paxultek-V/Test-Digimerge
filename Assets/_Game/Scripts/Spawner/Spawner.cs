using System;
using UnityEngine;

public enum SpawnerType
{
    ChargeButton,
    Automatic
}

public class Spawner : MonoBehaviour
{
    public static System.Action<ValueActor_Value, Vector3, float, float> OnSpawnValue;

    [SerializeField] private SpawnerType m_spawnerType;

    [SerializeField] private ValueActor_Value m_valuePrefab = null;

    [SerializeField] private GameObject m_spawnPosition = null;

    [SerializeField] private float m_maxEjectionStrength = 250f;

    [SerializeField] private Transform m_valuesParent = null;

    [SerializeField] private float m_initialValue = 1f;

    [SerializeField] private float m_spawnSpeed = 1f;


    private ValueActor_Value m_valueBuffer;
    private float m_spawnTimer;

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
    }

    private void Start()
    {
        m_isInCooldown = false;
    }

    private void Update()
    {
        if (m_isInCooldown)
        {
            m_spawnTimer += Time.deltaTime;
            if (m_spawnTimer > 1 / m_spawnSpeed)
                m_isInCooldown = false;
            return;
        }

        if (m_isContinuousSpawningEnabled == false)
            return;

        SpawnValue(1f);
        m_isInCooldown = true;
        m_spawnTimer = 0f;
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


    private void SpawnValue(float chargeProgression)
    {
        m_valueBuffer = Instantiate(m_valuePrefab, m_spawnPosition.transform.position, Quaternion.identity,
            m_valuesParent);

        OnSpawnValue?.Invoke(m_valueBuffer, m_spawnPosition.transform.forward, m_initialValue,
            m_maxEjectionStrength * chargeProgression);
    }
}