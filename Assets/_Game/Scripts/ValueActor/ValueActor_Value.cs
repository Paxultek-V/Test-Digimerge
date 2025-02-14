using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ValueActor_Value : MonoBehaviour
{
    public static Action<ValueBumper> OnValueBump;
    public Action<int> OnSendValueLength;
    
    [SerializeField] private Rigidbody m_body = null;

    [SerializeField] private float m_force = 1f;
    
    [SerializeField] private float m_minRotationForce = -150f;
    [SerializeField] private float m_maxRotationForce = 150f;
    
    [SerializeField] private TMP_Text m_text = null;

    [SerializeField] private Transform m_valueVisual = null;

    
    private Tweener m_tweener;
    private ValueBumper m_valueBumperBuffer;
    private float m_value;
    private readonly float m_collisionCooldownDuration = 0.1f;
    private float m_collisionCooldownTimer;
    private bool m_isInCooldownCollision;
    
    private void OnEnable()
    {
        Spawner.OnSpawnValue += OnSpawnValue;
    }

    private void OnDisable()
    {
        Spawner.OnSpawnValue -= OnSpawnValue;
    }


    private void Update()
    {
        if (m_isInCooldownCollision)
        {
            m_collisionCooldownTimer += Time.deltaTime;

            if (m_collisionCooldownTimer > m_collisionCooldownDuration)
            {
                m_isInCooldownCollision = false;
                m_collisionCooldownTimer = 0f;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(m_isInCooldownCollision)
            return;
        
        m_valueBumperBuffer = other.collider.gameObject.GetComponent<ValueBumper>();
        
        if(m_valueBumperBuffer == null)
            return;

        ApplyBumperEffect(m_valueBumperBuffer.BumpEffect);

        OnValueBump?.Invoke(m_valueBumperBuffer);

        m_isInCooldownCollision = true;
        
        m_valueBumperBuffer = null;
    }

    private void ApplyBumperEffect(BumpEffect bumpEffect)
    {
        switch (bumpEffect.ValueBumpEffect)
        {
            case ValueBumpEffect.Add:
                SetValue(m_value + bumpEffect.BumpValue);
                break;
            case ValueBumpEffect.Multiply:
                SetValue(m_value * bumpEffect.BumpValue);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        m_valueVisual.localScale = Vector3.one;
        
        if(m_tweener != null && m_tweener.IsPlaying())
            return;
        
        m_tweener = m_valueVisual.DOPunchScale(Vector3.one, 0.33f, 1);
    }

    private void OnSpawnValue(ValueActor_Value actorValue, Vector3 ejectionDirection, float value, float ejectionStrength)
    {
        if(actorValue != this)
            return;
        
        SetValue(value);

        AddForce(ejectionDirection, ejectionStrength);
    }
    

    private void AddForce(Vector3 direction, float strength)
    {
        m_body.AddForce(direction * strength);
        m_body.AddTorque(Vector3.forward * Random.Range(m_minRotationForce, m_maxRotationForce));
    }


    private void SetValue(float newValue)
    {
        m_value = newValue;
        UpdateText();
    }

    private void UpdateText()
    {
        m_text.text = "$" + m_value.ToString("F0");
        OnSendValueLength?.Invoke(m_text.text.Length);
    }

    public string FormatText()
    {
        string value = "";

        if (m_value < 10)
            value = m_value.ToString("F1");
        else
            value = m_value.ToString("F0");

        return "$" + value;
    }
}
