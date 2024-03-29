using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_ValueActor : MonoBehaviour
{
    public static Action<ValueActor_Value, Vector3, float, float> OnSpawnValue;
    public static Action OnAllValuesUsed;
    public static Action OnAllValuesSpawned;
    public Action<float> OnSendRemainingValueToSpawn;

    [Header("References")] [SerializeField]
    private Spawner_Value_SO m_spawnerData = null;

    [SerializeField] private Spawner_SpawningCondition m_spawnerSpawningCondition = null;
    [SerializeField] private Spawner_Stats m_spawnerStats = null;
    [SerializeField] private Transform m_valuesParent = null;
    [SerializeField] private GameObject m_spawnPosition = null;

    [Header("Specific parameters")] [SerializeField]
    private float m_initialAmountToSpawn = 100;

    private List<ValueActor_Value> m_valueActorList = new List<ValueActor_Value>();
    private ValueActor_Value m_valueBuffer;

    private float m_remainingValueToSpawn;
    private float m_additionalTimeBeforeSpawnStopsTimer;
    private bool m_canTrackRemainingValues;


    private void OnEnable()
    {
        Controller_LevelSection.OnNextLevelSectionLoaded += OnNextLevelSectionLoaded;

        ValueActor_Value.OnHitSplitter += OnHitSplitter;

        PiggyBank.OnPiggyBankFinishedCollectingMoney += OnPiggyBankFinishedCollectingMoney;

        ValueActor_Value.OnValueKilled += OnValueKilled;

        m_spawnerSpawningCondition.OnAdditionalTimeBeforeSpawnStopsOver += OnAdditionalTimeBeforeSpawnStopsOver;
    }

    private void OnDisable()
    {
        Controller_LevelSection.OnNextLevelSectionLoaded -= OnNextLevelSectionLoaded;

        ValueActor_Value.OnHitSplitter -= OnHitSplitter;

        PiggyBank.OnPiggyBankFinishedCollectingMoney -= OnPiggyBankFinishedCollectingMoney;

        ValueActor_Value.OnValueKilled -= OnValueKilled;

        m_spawnerSpawningCondition.OnAdditionalTimeBeforeSpawnStopsOver -= OnAdditionalTimeBeforeSpawnStopsOver;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        m_remainingValueToSpawn = m_initialAmountToSpawn;
        OnSendRemainingValueToSpawn?.Invoke(m_remainingValueToSpawn);
    }

    private void Update()
    {
        ManageSpawningFromCanon();
    }

    private void ManageSpawningFromCanon()
    {
        if (m_spawnerSpawningCondition.CanSpawn == false)
            return;

        if (m_remainingValueToSpawn <= 0)
            return;

        SpawnValue(m_spawnPosition.transform.position, m_spawnPosition.transform.forward,
            m_spawnerStats.CurrentValueToSpawn,
            1f);


        m_remainingValueToSpawn -= m_spawnerStats.CurrentValueToSpawn;
        OnSendRemainingValueToSpawn?.Invoke(m_remainingValueToSpawn);

        m_spawnerSpawningCondition.EnterCooldown(1 / m_spawnerStats.CurrentSpawnSpeed);

        if (m_remainingValueToSpawn <= 0)
        {
            OnAllValuesSpawned?.Invoke();

            if (m_canTrackRemainingValues == false)
                m_canTrackRemainingValues = true;
        }
    }

    private void OnAdditionalTimeBeforeSpawnStopsOver()
    {
        m_remainingValueToSpawn = 0;

        m_canTrackRemainingValues = true;

        if (m_canTrackRemainingValues && m_valueActorList.Count == 0)
            OnAllValuesUsed?.Invoke();

        OnSendRemainingValueToSpawn?.Invoke(m_remainingValueToSpawn);
    }

    private void OnHitSplitter(Vector3 splitPosition, float value)
    {
        SpawnValue(splitPosition, (Vector3.up + Vector3.left).normalized, (int)(value / 2f),
            m_spawnerData.splitEjectionStrength);
        SpawnValue(splitPosition, (Vector3.up + Vector3.right).normalized, (int)(value / 2f),
            m_spawnerData.splitEjectionStrength);
    }

    private void SpawnValue(Vector3 spawnPosition, Vector3 ejectionDirection, float value, float chargeProgression)
    {
        m_valueBuffer = Instantiate(m_spawnerData.valuePrefab, spawnPosition, Quaternion.identity, m_valuesParent);

        m_valueActorList.Add(m_valueBuffer);

        OnSpawnValue?.Invoke(m_valueBuffer, ejectionDirection, value,
            m_spawnerData.maxEjectionStrength * chargeProgression);
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

    private void OnPiggyBankFinishedCollectingMoney(float amountCollected, bool isLastPiggyBankOfLevel)
    {
        if (isLastPiggyBankOfLevel)
            return;

        m_remainingValueToSpawn = amountCollected;
        OnSendRemainingValueToSpawn?.Invoke(m_remainingValueToSpawn);
        m_canTrackRemainingValues = false;
    }

    private void OnNextLevelSectionLoaded()
    {
        m_canTrackRemainingValues = false;
    }
}