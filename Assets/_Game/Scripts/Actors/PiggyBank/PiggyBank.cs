using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PiggyBank : MonoBehaviour
{
    public static Action<float> OnPiggyBankFinishedCollectingMoney;
    public static Action<int> OnGrantGemReward;
    public static Action OnTargetAmountNotReached;
    public static Action OnTargetAmountReached;

    [SerializeField] private float m_amountToCollect = 100f;

    [SerializeField] private int m_gemsReward = 1;

    [SerializeField] private Transform m_visualToBump = null;

    [SerializeField] private Transform m_scaleController = null;

    [SerializeField] private TMP_Text m_textValue = null;


    private Coroutine m_delayCoroutine;
    private Tweener m_tweener;
    private float m_collectedAmount;

    private float m_progression
    {
        get => m_collectedAmount / m_amountToCollect;
    }

    private bool m_isTargetAmountReached;

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
        UpdateText();
        UpdateScale();
    }


    private void OnAllValuesUsed()
    {
        if (m_isTargetAmountReached)
        {
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

        if (m_isTargetAmountReached)
            return;

        CheckTargetAmountReachedCondition();
    }

    private void CheckTargetAmountReachedCondition()
    {
        if (m_collectedAmount >= m_amountToCollect)
        {
            m_isTargetAmountReached = true;
            OnTargetAmountReached?.Invoke();
            DestroyPiggyBank();
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
        m_textValue.text = m_collectedAmount.ToString("F0") + "/" + m_amountToCollect.ToString("F0");
    }
}