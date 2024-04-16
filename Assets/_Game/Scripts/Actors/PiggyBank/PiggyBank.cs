using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PiggyBank : MonoBehaviour
{
    public static Action<float> OnPiggyBankFinishedCollectingMoney;
    public static Action<int> OnGrantGemReward;
    public static Action<int> OnBroadcastStarsInfo;
    public static Action<float> OnBroadcastProgressionTowardsNextStar;
    public static Action<float> OnSendTotalMoneyCollected;
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
            OnSendTotalMoneyCollected?.Invoke(m_collectedAmount);
            OnTargetAmountReached?.Invoke();
            DestroyPiggyBank();
        }
        else
        {
            OnSendTotalMoneyCollected?.Invoke(m_collectedAmount);
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

        if (m_starsUnlocked >= 3)
            return;
        
        OnBroadcastProgressionTowardsNextStar?.Invoke(CalculateProgressionTowardsNextStar());
    }

    private void CheckTargetAmountReachedCondition()
    {
        if (m_starsUnlocked >= m_amountThresholdList.Count)
            return;


        int starsToUnlock = DetermineStarsToUnlock();

        for (int i = 0; i < starsToUnlock; i++)
        {
            UnlockStar();
        }
    }

    private int DetermineStarsToUnlock()
    {
        for (int i = m_amountThresholdList.Count - 1; i >= 0; i--)
        {
            if (m_collectedAmount > m_amountThresholdList[i])
            {
                return (i - m_starsUnlocked) + 1;
            }
        }

        return 0;
    }

    private void UnlockStar()
    {
        m_starsUnlocked++;
        m_starsUnlocked = Mathf.Clamp(m_starsUnlocked, 0, 3);

        OnBroadcastStarsInfo?.Invoke(m_starsUnlocked);
    }

    private float CalculateProgressionTowardsNextStar()
    {
        int previousIndex = m_starsUnlocked - 1;
        int nextIndex = m_starsUnlocked;

        float previous = 0;
        if (previousIndex >= 0)
            previous = m_amountThresholdList[previousIndex];
        
        float next = m_amountThresholdList[nextIndex];
        
        float progression = (m_collectedAmount - previous) / (next - previous);
        
        return progression;
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