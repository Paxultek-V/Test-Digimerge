using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Stars : MonoBehaviour
{
    [SerializeField] private List<Image> m_starList = null;

    private Image m_currentImage;
    private Tweener m_tweener;
    private int m_unlockedStarsCount;


    private void OnEnable()
    {
        PiggyBank.OnBroadcastStarsInfo += OnBroadcastStarsInfo;
        PiggyBank.OnBroadcastProgressionTowardsNextStar += OnBroadcastProgressionTowardsNextStar;
        Manager_GameState.OnBroadcastGameState += OnBroadcastGameState;
    }

    private void OnDisable()
    {
        PiggyBank.OnBroadcastStarsInfo -= OnBroadcastStarsInfo;
        PiggyBank.OnBroadcastProgressionTowardsNextStar -= OnBroadcastProgressionTowardsNextStar;
        Manager_GameState.OnBroadcastGameState -= OnBroadcastGameState;
    }

    private void Start()
    {
        Reset();
    }

    private void OnBroadcastGameState(GameState state)
    {
        Reset();
    }

    private void Reset()
    {
        m_unlockedStarsCount = 0;
        m_currentImage = m_starList[m_unlockedStarsCount];

        for (int i = 0; i < m_starList.Count; i++)
        {
            m_starList[i].fillAmount = 0;
        }
    }

    private void OnBroadcastProgressionTowardsNextStar(float progression)
    {
        m_currentImage.fillAmount = progression;

        if (m_tweener != null && m_tweener.IsPlaying())
            return;

        m_tweener = m_currentImage.transform.DOPunchScale(Vector3.one / 2f, 0.33f, 1)
            .OnComplete(() => m_currentImage.transform.localScale = Vector3.one);
    }

    private void OnBroadcastStarsInfo(int unlockedStars)
    {
        int diff = unlockedStars - m_unlockedStarsCount;

        for (int i = 0; i < diff; i++)
        {
            UnlockStar();
        }
    }

    private void UnlockStar()
    {
        Image star = m_starList[m_unlockedStarsCount];

        m_currentImage.fillAmount = 1;
        m_currentImage.transform.localScale = Vector3.one;
        
        m_unlockedStarsCount++;

        if (m_tweener != null && m_tweener.IsPlaying())
        {
            m_currentImage.transform.localScale = Vector3.one;
            m_tweener = null;
        }

        if (star == null)
            return;

        star.transform.DOPunchScale(Vector3.one, 0.33f, 1)
            .OnComplete(() => star.transform.DORotate(Vector3.forward * 719, 1f, RotateMode.FastBeyond360));
        
        if(m_unlockedStarsCount >= 3)
            return;
        
        m_currentImage = m_starList[m_unlockedStarsCount];

    }
}