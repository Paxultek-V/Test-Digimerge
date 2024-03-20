using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ValueActor_ScaleController : MonoBehaviour
{
    [SerializeField] private ValueActor_Value m_valueActorValue = null;
    
    [SerializeField] private BoxCollider m_collider = null;

    [SerializeField] private RectTransform m_rectTransform = null;
    
    [SerializeField] private Vector3 m_valueActorScale = Vector3.zero;

    [SerializeField] private float m_characterSizeRatio = 0.75f;


    private void OnEnable()
    {
        m_valueActorValue.OnSendValueLength += OnSendValueLength;
    }

    private void OnDisable()
    {
        m_valueActorValue.OnSendValueLength -= OnSendValueLength;
    }

    private void Update()
    {
        if(m_collider == null || m_rectTransform == null)
            return;

        UpdateScale();

    }

    private void OnSendValueLength(int valueLength)
    {
        m_valueActorScale.x = valueLength * m_characterSizeRatio;
    }

    private void UpdateScale()
    {
        m_collider.size = m_valueActorScale;
        m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_valueActorScale.x);
        m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_valueActorScale.y);

    }
}
