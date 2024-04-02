using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Debug : MonoBehaviour
{
    [SerializeField] private Animator m_animator = null;

    private bool m_isPanelOpen;

    private void Start()
    {
        m_isPanelOpen = false;
        m_animator.SetTrigger("Disappear");
    }


    public void TogglePanel()
    {
        m_isPanelOpen = !m_isPanelOpen;

        if (m_isPanelOpen)
            m_animator.SetTrigger("Appear");
        else
            m_animator.SetTrigger("Disappear");
    }
}