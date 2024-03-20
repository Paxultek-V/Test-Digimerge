using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerChargingButton : MonoBehaviour
{
    public static Action<float> OnFinishedCharging;

    [SerializeField] private Slider m_chargeSlider = null;
    
    [SerializeField] private float m_chargeSpeed = 0.5f;
    
    public float m_currentCharge;
    private float m_timer;
    private bool m_isCharging;

    private void Start()
    {
        m_currentCharge = 0;
        m_isCharging = false;
        UpdateSlider();
    }

    private void Update()
    {
        if (m_isCharging)
        {
            m_timer += Time.deltaTime * m_chargeSpeed;
            m_currentCharge = Mathf.PingPong(m_timer,1f);
            UpdateSlider();
        }
    }

    private void UpdateSlider()
    {
        m_chargeSlider.value = m_currentCharge;
    }

    public void StartCharging()
    {
        m_currentCharge = 0f;
        m_timer = 0f;
        m_isCharging = true;
    }

    public void StopCharging()
    {
        m_isCharging = false;
        
        OnFinishedCharging?.Invoke(m_currentCharge);
    }
    
}
