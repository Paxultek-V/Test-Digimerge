using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public enum BonusType
{
    SpawnFrequency,
    BoostInitialValue
}

[ExecuteAlways]
public class ValueBonus : MonoBehaviour
{
    public Action OnBonusKilled;
    public static Action<BonusType, float, float> OnGrantBonus;

    [Header("Parameters")]
    [SerializeField] private BonusType m_type;
    [SerializeField] private float m_bonusValue = 1;
    [SerializeField] private float m_bonusDuration = 30;
    [SerializeField] private float m_health = 250f;
    
    [Header("References")]
    [SerializeField] private TMP_Text m_valueText = null;
    [SerializeField] private Transform m_transformToBump = null;

    
    private Tweener m_tweener;
    private float m_currentValueBonus;
    

    private void OnEnable()
    {
        ValueActor_Value.OnHitValueBonus += OnHitValueBonus;
    }

    private void OnDisable()
    {
        ValueActor_Value.OnHitValueBonus -= OnHitValueBonus;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        m_currentValueBonus = m_health;

        UpdatePlatformValueText();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            m_currentValueBonus = m_health;
            UpdatePlatformValueText();
        }
#endif
    }

    private void OnHitValueBonus(ValueBonus valueBonus, float value)
    {
        if (valueBonus != this)
            return;

        DecreaseValue(value);

        m_transformToBump.localScale = Vector3.one;

        if (m_tweener != null && m_tweener.IsPlaying())
            return;

        m_tweener = m_transformToBump.DOPunchScale(Vector3.one, 0.33f, 1);
    }

    private void DecreaseValue(float value)
    {
        m_currentValueBonus -= value;

        UpdatePlatformValueText();

        if (m_currentValueBonus <= 0)
        {
            OnGrantBonus?.Invoke(m_type, m_bonusValue, m_bonusDuration);
            Kill();
        }
    }

    private void Kill()
    {
        OnBonusKilled?.Invoke();
        Destroy(gameObject);
    }

    private void UpdatePlatformValueText()
    {
        m_valueText.text = m_currentValueBonus.ToString("F0");
    }
}