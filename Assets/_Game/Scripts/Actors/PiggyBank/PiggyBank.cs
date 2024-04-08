using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PiggyBank : MonoBehaviour
{
    public static Action<float> OnPiggyBankFinishedCollectingMoney;
    public static Action<int> OnGrantGemReward;
    public static Action<int> OnBroadcastStars;
    public static Action OnTargetAmountNotReached;
    public static Action OnTargetAmountReached;

    [SerializeField] private List<float> m_amountThresholdList = null;

    [SerializeField] private int m_gemsReward = 1;

    [SerializeField] private Transform m_visualToBump = null;

    [SerializeField] private Transform m_scaleController = null;

    [SerializeField] private TMP_Text m_textValue = null;


    private Coroutine m_delayCoroutine;
    private Tweener m_tweener;
    private float m_collectedAmount;
    private int m_starsUnlocked;

    private float m_progression
    {
        get => m_collectedAmount / m_amountThresholdList[^1];
    }

    private void OnEnable()
    {
        ValueActor_Value.OnHitPiggyBank += OnHitPiggyBank;
        Spawner_ValueActor.OnAllValuesUsed += OnAllValuesUsed;
    }

    private void OnDisable()
    {
        ValueActor_Value.OnHitPiggyBank -= OnHitPiggyBank;
        Spawner_ValueActor.OnAllValuesUsed -= OnAllValuesUsed;
    }

    private void Start()
    {
        m_collectedAmount = 0;
        m_starsUnlocked = 0;
        UpdateText();
        UpdateScale();
    }


    private void OnAllValuesUsed()
    {
        if (m_starsUnlocked > 0)
        {
            OnTargetAmountReached?.Invoke();
            DestroyPiggyBank();
        }
        else
        {
            OnTargetAmountNotReached?.Invoke();
        }
    }

    private void DestroyPiggyBank()
    {
        OnGrantGemReward?.Invoke(m_gemsReward);

        OnPiggyBankFinishedCollectingMoney?.Invoke(m_collectedAmount);

        Destroy(gameObject);
    }

    private void OnHitPiggyBank(float value)
    {
        m_collectedAmount += value;

        UpdateText();

        UpdateScale();

        PlayTweenAnimation();

        CheckTargetAmountReachedCondition();
    }

    private void CheckTargetAmountReachedCondition()
    {
        if (m_collectedAmount >= m_amountThresholdList[m_starsUnlocked])
        {
            m_starsUnlocked++;
            m_starsUnlocked = Mathf.Clamp(m_starsUnlocked, 0, 3);

            OnBroadcastStars?.Invoke(m_starsUnlocked);
        }
    }

    private void PlayTweenAnimation()
    {
        m_visualToBump.localScale = Vector3.one;

        if (m_tweener != null && m_tweener.IsPlaying())
            return;

        m_tweener = m_visualToBump.DOPunchScale(Vector3.one / 2f, 0.33f, 1);
    }

    private void UpdateScale()
    {
        m_scaleController.localScale = Vector3.one + Vector3.one * Mathf.Clamp01(m_progression);
    }

    private void UpdateText()
    {
        if (m_collectedAmount == 0)
        {
            m_textValue.text = "$0";
        }

        m_textValue.text = "$" + m_collectedAmount.FormatNumber();
    }
}