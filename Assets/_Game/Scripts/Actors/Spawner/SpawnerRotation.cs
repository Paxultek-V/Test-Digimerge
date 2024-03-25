using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerRotation : MonoBehaviour
{
    [SerializeField] private Transform m_controlledTransform = null;
    
    [SerializeField] private float m_minAngle = 10f;

    [SerializeField] private float m_maxAngle = 30f;

    [SerializeField] private float m_rotationSpeed = 20f;

    private float m_currentRotation;
    private float m_timer;

    private void Start()
    {
        m_currentRotation = m_minAngle;
    }

    private void Update()
    {
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        m_timer += Time.deltaTime * m_rotationSpeed;

        m_currentRotation = m_minAngle + Mathf.PingPong(m_timer, m_maxAngle - m_minAngle);

        m_controlledTransform.rotation = Quaternion.Euler(0f, 0f, m_currentRotation);
    }
}
