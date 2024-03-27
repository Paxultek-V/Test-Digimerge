using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Controller_LevelSection : MonoBehaviour
{
    public static Action OnStartLoadingNextSectionLevel;
    public static Action OnNextLevelSectionLoaded;
    public static Action OnFinishedLevel;

    [SerializeField] private List<GameObject> m_levelSectionList = null;

    [SerializeField] private GameObject m_levelSectionSpawnPosition = null;
    [SerializeField] private GameObject m_levelSectionGameplayPosition = null;
    [SerializeField] private GameObject m_levelSectionDestroyPosition = null;

    [SerializeField] private Transform m_levelSectionParent = null;

    [SerializeField] private float m_levelSectionTransitionDuration = 2f;

    private GameObject m_currentLevelSection;
    private GameObject m_levelSectionToDestroy;
    private int m_indexLevelSectionToLoad;
    private bool m_isLastLevelSection;

    private void OnEnable()
    {
        PiggyBank.OnPiggyBankFinishedCollectingMoney += OnPiggyBankFinishedCollectingMoney;
    }

    private void OnDisable()
    {
        PiggyBank.OnPiggyBankFinishedCollectingMoney -= OnPiggyBankFinishedCollectingMoney;
    }


    private void Start()
    {
        m_indexLevelSectionToLoad = 0;
        LoadNextLevelSection(false);
    }


    private void LoadNextLevelSection(bool withSpawnAnimation)
    {
        BroadcastStartLoadingNextLevelSection();

        Vector3 spawnPosition = Vector3.zero;

        if (withSpawnAnimation)
            spawnPosition = m_levelSectionSpawnPosition.transform.position;
        else
            spawnPosition = m_levelSectionGameplayPosition.transform.position;

        m_currentLevelSection = Instantiate(m_levelSectionList[m_indexLevelSectionToLoad], spawnPosition,
            Quaternion.identity, m_levelSectionParent);

        if (withSpawnAnimation)
            m_currentLevelSection.transform.DOMove(m_levelSectionGameplayPosition.transform.position, m_levelSectionTransitionDuration)
                .OnComplete(BroadcastOnNextLevelSectionLoaded);
        else
            BroadcastOnNextLevelSectionLoaded();

        m_indexLevelSectionToLoad++;

        if (m_indexLevelSectionToLoad >= m_levelSectionList.Count)
            m_isLastLevelSection = true;
    }

    private void ManageCompletedLevelSection()
    {
        if (m_currentLevelSection != null)
        {
            m_levelSectionToDestroy = m_currentLevelSection;

            m_levelSectionToDestroy.transform.DOMove(m_levelSectionDestroyPosition.transform.position, m_levelSectionTransitionDuration)
                .OnComplete(() => Destroy(m_levelSectionToDestroy));
        }
    }

    private void BroadcastStartLoadingNextLevelSection()
    {
        OnStartLoadingNextSectionLevel?.Invoke();
    }

    private void BroadcastOnNextLevelSectionLoaded()
    {
        OnNextLevelSectionLoaded?.Invoke();
    }

    private void OnPiggyBankFinishedCollectingMoney(float amountCollected, bool isLastPiggyBankOfLevel)
    {
        if (m_isLastLevelSection)
        {
            OnFinishedLevel?.Invoke();
            return;
        }
        
        ManageCompletedLevelSection();
        LoadNextLevelSection(true);
    }
}