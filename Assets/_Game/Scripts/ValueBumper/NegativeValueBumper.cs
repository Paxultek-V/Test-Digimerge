using DG.Tweening;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class NegativeValueBumper : MonoBehaviour
{
    [SerializeField] private BumpEffect m_bumpEffect = null;

    [SerializeField] private TMP_Text m_bumpEffectText = null;

    [SerializeField] private Transform m_bumperVisual = null;

    private Tweener m_tweener;

    public BumpEffect BumpEffect
    {
        get => m_bumpEffect;
    }

    
    private void OnEnable()
    {
        ValueActor_Value.OnHitNegativeValueBumper += OnHitNegativeValueBumper;
    }

    private void OnDisable()
    {
        ValueActor_Value.OnHitNegativeValueBumper -= OnHitNegativeValueBumper;
    }

    private void Start()
    {
        UpdateBumpEffectText();
    }

    private void Update()
    {
#if UNITY_EDITOR
        UpdateBumpEffectText();
#endif
    }

    private void UpdateBumpEffectText()
    {
        if (m_bumpEffectText == null)
            return;

        m_bumpEffectText.text = m_bumpEffect.BumpValue.ToString("F0");
    }


    private void OnHitNegativeValueBumper(NegativeValueBumper negativeValueBumper)
    {
        if (negativeValueBumper != this)
            return;

        m_bumperVisual.localScale = Vector3.one;

        if (m_tweener != null && m_tweener.IsPlaying())
            return;

        m_tweener = m_bumperVisual.DOPunchScale(Vector3.one, 0.33f, 1);
    }
}