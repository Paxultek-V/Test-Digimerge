using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public enum BonusType
{
    SpawnFrequency,
    BoostInitialValue
}

[ExecuteAlways]
public class ValueBonus : MonoBehaviour
{
    public static Action<BonusType, float, float> OnGrantBonus;

    [SerializeField] private BonusType m_type;
    [SerializeField] private float m_bonusValue = 1;
    [SerializeField] private float m_bonusDuration = 30;

    [SerializeField] private float m_health = 250f;
    
    [SerializeField] private GameObject m_spawnFrequencyFxGroup = null;
    [SerializeField] private GameObject m_boostInitialValueFxGroup = null;
    [SerializeField] private TMP_Text m_valueText = null;
    [SerializeField] private Transform m_transformToBump = null;


    
    private float m_currentValueBonus;

    private Tweener m_tweener;

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
        m_currentValueBonus = m_health;

        m_spawnFrequencyFxGroup.SetActive(m_type == BonusType.SpawnFrequency);
        m_boostInitialValueFxGroup.SetActive(m_type == BonusType.BoostInitialValue);
        
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
        Destroy(gameObject);
    }

    private void UpdatePlatformValueText()
    {
        m_valueText.text = m_currentValueBonus.ToString("F0");
    }
}