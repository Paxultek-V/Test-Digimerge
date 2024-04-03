using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum ValueBumpEffect
{
    Add,
    Multiply,
    Subtract
}

[Serializable]
public class BumpEffect
{
    [SerializeField] public ValueBumpEffect ValueBumpEffect;

    [SerializeField] public float BumpValue = 1f;
}

[Serializable]
public class MeshMaterialData
{
    public MeshRenderer MeshRenderer;
    public Material AddEffectMaterial;
    public Material MultiplyEffectMaterial;
    public Material SubtractEffectMaterial;

    public void UpdateMaterial(ValueBumpEffect effect)
    {
        switch (effect)
        {
            case ValueBumpEffect.Add:
                MeshRenderer.material = AddEffectMaterial;
                break;
            case ValueBumpEffect.Multiply:
                MeshRenderer.material = MultiplyEffectMaterial;
                break;
            case ValueBumpEffect.Subtract:
                MeshRenderer.material = SubtractEffectMaterial;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(effect), effect, null);
        }
    }
}

[ExecuteAlways]
public class ValueBumper : MonoBehaviour
{
    [Header("Bump effect parameters")] [SerializeField]
    private BumpEffect m_bumpEffect = null;

    [Header("Model visual")] [SerializeField]
    private List<MeshMaterialData> m_meshMaterialDataList = null;

    [Header("Light visual")] [SerializeField]
    private Image m_fakeLightImage = null;

    [SerializeField] private Color m_addEffectColor = Color.white;
    [SerializeField] private Color m_multiplyEffectColor = Color.white;
    [SerializeField] private Color m_subtractEffectColor = Color.white;

    [Header("References")] [SerializeField]
    private TMP_Text m_bumpEffectText = null;

    [SerializeField] private Transform m_bumperVisual = null;


    private Tweener m_tweener;

    public BumpEffect BumpEffect
    {
        get => m_bumpEffect;
    }

    private void Start()
    {
        UpdateLight();
        UpdateEffectMaterial();
        UpdateBumpEffectText();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
            return;

        UpdateLight();
        UpdateEffectMaterial();
        UpdateBumpEffectText();
#endif
    }

    private void OnEnable()
    {
        ValueActor_Value.OnHitValueBumper += OnValueBump;
    }

    private void OnDisable()
    {
        ValueActor_Value.OnHitValueBumper -= OnValueBump;
    }

    private void UpdateLight()
    {
        switch (m_bumpEffect.ValueBumpEffect)
        {
            case ValueBumpEffect.Add:
                m_fakeLightImage.color = m_addEffectColor;
                break;
            case ValueBumpEffect.Multiply:
                m_fakeLightImage.color = m_multiplyEffectColor;
                break;
            case ValueBumpEffect.Subtract:
                m_fakeLightImage.color = m_subtractEffectColor;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void UpdateBumpEffectText()
    {
        if (m_bumpEffectText == null)
            return;

        switch (m_bumpEffect.ValueBumpEffect)
        {
            case ValueBumpEffect.Add:
                m_bumpEffectText.text = "+" + m_bumpEffect.BumpValue.ToString("F0");
                break;
            case ValueBumpEffect.Multiply:
                m_bumpEffectText.text = "x" + m_bumpEffect.BumpValue.ToString("F0");
                break;
            case ValueBumpEffect.Subtract:
                m_bumpEffectText.text = "-" + m_bumpEffect.BumpValue.ToString("F0");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateEffectMaterial()
    {
        if (m_meshMaterialDataList == null || m_meshMaterialDataList.Count == 0)
            return;

        for (int i = 0; i < m_meshMaterialDataList.Count; i++)
        {
            m_meshMaterialDataList[i].UpdateMaterial(m_bumpEffect.ValueBumpEffect);
        }
    }

    private void OnValueBump(ValueBumper valueBumper)
    {
        if (valueBumper != this)
            return;

        m_bumperVisual.localScale = Vector3.one;

        if (m_tweener != null && m_tweener.IsPlaying())
            return;

        m_tweener = m_bumperVisual.DOPunchScale(Vector3.one, 0.33f, 1);
    }
}