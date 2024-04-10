using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UI_Victory : MonoBehaviour
{
    [SerializeField] private List<Transform> m_starList = null;

    private int m_unlockedStars;


    private void OnEnable()
    {
        PiggyBank.OnBroadcastStars += OnBroadcastStars;
        Manager_GameState.OnBroadcastGameState += OnBroadcastGameState;
    }

    private void OnDisable()
    {
        PiggyBank.OnBroadcastStars -= OnBroadcastStars;
        Manager_GameState.OnBroadcastGameState -= OnBroadcastGameState;
    }


    private void Initialize()
    {
        m_unlockedStars = 0;

        for (int i = 0; i < m_starList.Count; i++)
        {
            m_starList[i].gameObject.SetActive(false);
        }
    }

    private void OnBroadcastStars(int unlockedStars)
    {
        m_unlockedStars = unlockedStars;
    }


    private void OnBroadcastGameState(GameState state)
    {
        if (state == GameState.InGame)
            Initialize();

        if (state == GameState.Victory)
            StartCoroutine(StarsAnimationCoroutine());
    }

    private IEnumerator StarsAnimationCoroutine()
    {
        yield return new WaitForSecondsRealtime(1f);
        
        for (int i = 0; i < m_unlockedStars; i++)
        {
            if (i >= m_starList.Count)
                yield break;

            m_starList[i].gameObject.SetActive(true);

            m_starList[i].DOPunchScale(Vector3.one / 2f, 0.33f, 1);

            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
}