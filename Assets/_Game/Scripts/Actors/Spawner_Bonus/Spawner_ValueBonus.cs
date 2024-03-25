using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner_ValueBonus : MonoBehaviour
{
    [SerializeField] private ValueBonus m_valueMoneyBonusPrefab = null;
    [SerializeField] private ValueBonus m_valueSpawnSpeedBonusPrefab = null;

    [SerializeField] private Transform m_bonusParent = null;

    [SerializeField] private float m_minTimeSpawnDelay = 15f;
    [SerializeField] private float m_maxTimeSpawnDelay = 30f;

    private List<Transform> m_spawnPositionList = new List<Transform>();
    private ValueBonus m_valueBonusBuffer;
    private ValueBonus m_valueBonusToSpawnBuffer;
    private float m_delayBuffer;
    private float m_timer;
    private bool m_canSpawn;
    private bool m_isSpawningInCooldown;

    private void Awake()
    {
        FillSpawnPositionList();
    }

    private void Start()
    {
        m_canSpawn = false;
        m_isSpawningInCooldown = true;
        m_timer = 0f;
        m_delayBuffer = Random.Range(m_minTimeSpawnDelay, m_maxTimeSpawnDelay);
    }

    private void Update()
    {
        ManageValueBonusSpawning();
    }

    private void FillSpawnPositionList()
    {
        foreach (Transform child in transform)
        {
            m_spawnPositionList.Add(child);
        }
    }

    private void ManageValueBonusSpawning()
    {
        if (m_isSpawningInCooldown)
        {
            m_timer += Time.deltaTime;

            if (m_timer > m_delayBuffer)
            {
                m_canSpawn = true;
                m_isSpawningInCooldown = false;
            }
        }

        if (m_canSpawn && !m_isSpawningInCooldown)
        {
            SpawnBonus();
        }
    }

    private void SpawnBonus()
    {
        if (m_spawnPositionList == null || m_spawnPositionList.Count == 0)
            return;

        m_valueBonusToSpawnBuffer =
            Random.Range(0f, 1f) < 0.5f ? m_valueMoneyBonusPrefab : m_valueSpawnSpeedBonusPrefab;

        m_valueBonusBuffer =
            Instantiate(m_valueBonusToSpawnBuffer, GetRandomSpawnPosition(), Quaternion.identity, m_bonusParent);

        m_canSpawn = false;

        m_valueBonusBuffer.OnBonusKilled += OnBonusKilled;
    }

    private void OnBonusKilled()
    {
        m_valueBonusBuffer.OnBonusKilled -= OnBonusKilled;
        m_isSpawningInCooldown = true;
        m_timer = 0f;
        m_delayBuffer = Random.Range(m_minTimeSpawnDelay, m_maxTimeSpawnDelay);
    }


    private Vector3 GetRandomSpawnPosition()
    {
        int randomIndex = Random.Range(0, m_spawnPositionList.Count);
        return m_spawnPositionList[randomIndex].position;
    }
}