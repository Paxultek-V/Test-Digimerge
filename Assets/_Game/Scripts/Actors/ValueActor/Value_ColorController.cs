using TMPro;
using UnityEngine;

public class Value_ColorController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private Gradient m_gradient = null;

    [SerializeField] private float m_maxValue = 10000;
    
    [Header("References")]
    [SerializeField] private TMP_Text m_text = null;

    private ValueActor_Value m_value;


    private void Awake()
    {
        m_value = GetComponent<ValueActor_Value>();
    }

    private void OnEnable()
    {
        m_value.OnSendValue += OnSendValue;
    }

    private void OnDisable()
    {
        m_value.OnSendValue -= OnSendValue;
    }


    private void OnSendValue(float value)
    {
        UpdateColor(value);
    }

    private void UpdateColor(float value)
    {
        float progression = value / m_maxValue;

        m_text.color = m_gradient.Evaluate(progression);
    }
}
