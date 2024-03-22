using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class PlatformValue : MonoBehaviour
{
    public static Action OnDestroyPlatformToNextLevel;
    
    [SerializeField] private float m_platformInitialValue = 50f;

    [SerializeField] private TMP_Text m_valueText = null;

    [SerializeField] private Transform m_transformToBump = null;

    [SerializeField] private bool m_isPlatformToNextLevel = false;
    
    private Tweener m_tweener;
    private float m_platformCurrentValue;


    private void OnEnable()
    {
        ValueActor_Value.OnHitPlatformValue += OnHitPlatformValue;
    }

    private void OnDisable()
    {
        ValueActor_Value.OnHitPlatformValue -= OnHitPlatformValue;
    }

    private void Start()
    {
        m_platformCurrentValue = m_platformInitialValue;
        UpdatePlatformValueText();
    }


    private void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            m_platformCurrentValue = m_platformInitialValue;
            UpdatePlatformValueText();
        }
#endif
    }

    private void OnHitPlatformValue(PlatformValue platformValue, float value)
    {
        if (platformValue != this)
            return;

        DecreaseValue(value);

        m_transformToBump.localScale = Vector3.one;

        if (m_tweener != null && m_tweener.IsPlaying())
            return;

        m_tweener = m_transformToBump.DOPunchScale(Vector3.one, 0.33f, 1);
    }


    private void DecreaseValue(float value)
    {
        m_platformCurrentValue -= value;

        UpdatePlatformValueText();

        if (m_platformCurrentValue <= 0)
            Kill();
    }

    private void Kill()
    {
        if(m_isPlatformToNextLevel)
            OnDestroyPlatformToNextLevel?.Invoke();
        
        Destroy(gameObject);
    }

    private void UpdatePlatformValueText()
    {
        m_valueText.text = m_platformCurrentValue.ToString("F0");
    }
}