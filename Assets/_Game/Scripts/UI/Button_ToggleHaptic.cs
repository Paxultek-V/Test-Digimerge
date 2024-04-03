using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Button_ToggleHaptic : MonoBehaviour
{
    public static Action OnToggleHaptic_ButtonPressed;

    [SerializeField] private Image m_buttonImage = null;

    [SerializeField] private TMP_Text m_buttonText = null;
    
    private Button m_button;


    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(ToggleHapticButton);
    }

    private void OnEnable()
    {
        Manager_HapticFeedback.OnToggleHaptic += OnToggleHaptic;
    }

    private void OnDisable()
    {
        Manager_HapticFeedback.OnToggleHaptic -= OnToggleHaptic;
    }


    private void OnToggleHaptic(bool isHapticEnabled)
    {
        m_buttonImage.color = isHapticEnabled ? Color.green : Color.red;
        m_buttonText.text = "Haptic\n" + (isHapticEnabled ? "On" : "Off");
    }
    
    private void ToggleHapticButton()
    {
        OnToggleHaptic_ButtonPressed?.Invoke();
    }
    
    
}
