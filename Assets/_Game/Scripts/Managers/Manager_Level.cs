using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Manager_Level : MonoBehaviour
{
    public static Action OnStartLoadingNextLevel;
    public static Action OnNextLevelLoaded;

    [SerializeField] private List<GameObject> m_levelList = null;

    [SerializeField] private GameObject m_levelSpawnPosition = null;
    [SerializeField] private GameObject m_levelGameplayPosition = null;
    [SerializeField] private GameObject m_levelDestroyPosition = null;

    [SerializeField] private Transform m_levelParent = null;

    [SerializeField] private float m_levelTransitionDuration = 2f;

    private GameObject m_currentLevel;
    private GameObject m_levelToDestroy;
    private int m_indexLevelToLoad;

    private void OnEnable()
    {
        PlatformValue.OnDestroyPlatformToNextLevel += OnDestroyPlatformToNextLevel;
    }

    private void OnDisable()
    {
        PlatformValue.OnDestroyPlatformToNextLevel -= OnDestroyPlatformToNextLevel;
    }


    private void Start()
    {
        m_indexLevelToLoad = 0;
        LoadNextLevel(false);
    }


    private void LoadNextLevel(bool withSpawnAnimation)
    {
        BroadcastStartLoadingNextLevel();

        Vector3 spawnPosition = Vector3.zero;

        if (withSpawnAnimation)
            spawnPosition = m_levelSpawnPosition.transform.position;
        else
            spawnPosition = m_levelGameplayPosition.transform.position;

        m_currentLevel = Instantiate(m_levelList[m_indexLevelToLoad], spawnPosition,
            Quaternion.identity, m_levelParent);

        if (withSpawnAnimation)
            m_currentLevel.transform.DOMove(m_levelGameplayPosition.transform.position, m_levelTransitionDuration)
                .OnComplete(BroadcastOnNextLevelLoaded);
        else
            BroadcastOnNextLevelLoaded();

        m_indexLevelToLoad++;

        if (m_indexLevelToLoad >= m_levelList.Count)
            m_indexLevelToLoad = 0;
    }

    private void ManageCompletedLevel()
    {
        if (m_currentLevel != null)
        {
            m_levelToDestroy = m_currentLevel;

            m_levelToDestroy.transform.DOMove(m_levelDestroyPosition.transform.position, m_levelTransitionDuration)
                .OnComplete(() => Destroy(m_levelToDestroy));
        }
    }

    private void BroadcastStartLoadingNextLevel()
    {
        OnStartLoadingNextLevel?.Invoke();
    }

    private void BroadcastOnNextLevelLoaded()
    {
        OnNextLevelLoaded?.Invoke();
    }

    private void OnDestroyPlatformToNextLevel()
    {
        ManageCompletedLevel();
        LoadNextLevel(true);
    }
}