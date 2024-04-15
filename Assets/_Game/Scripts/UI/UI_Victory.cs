using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UI_Victory : MonoBehaviour
{
    [SerializeField] private List<Transform> m_starList = null;

    [SerializeField] private TMP_Text m_victoryText = null;

    [SerializeField] private TMP_Text m_moneyEarnedText = null;

    [SerializeField] private GameObject m_tapToContinueButton = null;


    private int m_unlockedStars;


    private void OnEnable()
    {
        PiggyBank.OnBroadcastStarsInfo += OnBroadcastStars;
        PiggyBank.OnSendTotalMoneyCollected += OnSendTotalMoneyCollected;
        Manager_GameState.OnBroadcastGameState += OnBroadcastGameState;
    }

    private void OnDisable()
    {
        PiggyBank.OnBroadcastStarsInfo -= OnBroadcastStars;
        PiggyBank.OnSendTotalMoneyCollected -= OnSendTotalMoneyCollected;
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

    private void OnSendTotalMoneyCollected(float amountCollected)
    {
        m_moneyEarnedText.text = "You earned\n$" + amountCollected.FormatNumber();
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
            PlayVictoryUISequence();
    }


    private void PlayVictoryUISequence()
    {
        m_tapToContinueButton.SetActive(false);

        m_victoryText.transform.localScale = Vector3.one * 0.4f;
        m_moneyEarnedText.transform.localScale = Vector3.one * 0.4f;

        m_victoryText.transform.DOScale(Vector3.one, 0.3f).OnComplete(() =>
            m_moneyEarnedText.transform.DOScale(Vector3.one, 0.3f).OnComplete(() =>
                m_moneyEarnedText.transform.DOPunchScale(Vector3.one / 2f, 0.33f, 1).OnComplete(() =>
                    StartCoroutine(StarsAnimationCoroutine()))));
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

        m_tapToContinueButton.SetActive(true);
    }
}