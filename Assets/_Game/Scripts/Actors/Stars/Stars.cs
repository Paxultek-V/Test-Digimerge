using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Stars : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_starList = null;

    [SerializeField] private Transform m_starSpawnPosition = null;


    private Tweener m_tweener;
    private int m_currentStarsDisplayed;


    private void OnEnable()
    {
        PiggyBank.OnBroadcastStars += OnBroadcastStars;
    }

    private void OnDisable()
    {
        PiggyBank.OnBroadcastStars -= OnBroadcastStars;
    }

    private void Start()
    {
        m_currentStarsDisplayed = 0;

        for (int i = 0; i < m_starList.Count; i++)
        {
            m_starList[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UnlockStar();
        }
    }

    private void OnBroadcastStars(int unlockedStars)
    {
        int diff = unlockedStars - m_currentStarsDisplayed;
        m_currentStarsDisplayed = unlockedStars;

        for (int i = 0; i < diff; i++)
        {
            UnlockStar();
        }
    }

    private void UnlockStar()
    {
        GameObject star = GetStarToUnlock();

        if (star == null)
            return;

        Vector3 targetPosition = star.transform.position;
        
        star.SetActive(true);

        star.transform.position = m_starSpawnPosition.position;

        star.transform.DOPunchScale(Vector3.one / 2f, 0.33f, 1)
            .OnComplete(() => star.transform.DOMove(targetPosition, 0.5f)
            .OnComplete(() => star.transform.DOPunchScale(Vector3.one / 2f, 0.33f, 1)));
    }

    private GameObject GetStarToUnlock()
    {
        for (int i = 0; i < m_starList.Count; i++)
        {
            if (m_starList[i].activeInHierarchy == false)
                return m_starList[i];
        }

        return null;
    }
}