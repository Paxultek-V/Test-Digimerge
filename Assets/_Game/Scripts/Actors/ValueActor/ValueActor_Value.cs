using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ValueActor_Value : MonoBehaviour
{
    public static Action<ValueBumper> OnHitValueBumper;
    public static Action<PlatformValue, float> OnHitPlatformValue;
    public static Action<ValueBonus, float> OnHitValueBonus;
    public static Action<Vector3, float> OnHitSplitter;
    public Action<int> OnSendValueLength;

    [SerializeField] private Rigidbody m_body = null;
    [SerializeField] private TMP_Text m_text = null;
    [SerializeField] private Transform m_valueVisual = null;

    [SerializeField] private float m_minRotationForce = -150f;
    [SerializeField] private float m_maxRotationForce = 150f;
    [SerializeField] private readonly float m_collisionCooldownDuration = 0.2f;



    private Tweener m_tweener;
    private ValueBumper m_valueBumperBuffer;
    private BlackHole m_blackHoleBuffer;
    private PlatformValue m_platformValueBuffer;
    private ValueBonus m_valueBonusBuffer;
    private Splitter m_splitterBuffer;
    private float m_value;
    private float m_collisionCooldownTimer;
    private bool m_isInCooldownCollision;

    private void OnEnable()
    {
        Spawner_ValueActor.OnSpawnValue += OnSpawnValue;
    }

    private void OnDisable()
    {
        Spawner_ValueActor.OnSpawnValue -= OnSpawnValue;
    }


    private void Update()
    {
        ManageCollisionCooldown();
    }

    private void ManageCollisionCooldown()
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

    #region Interactions

    private void OnCollisionEnter(Collision other)
    {
        if (m_isInCooldownCollision)
            return;

        ManageInteractionWithValueBumper(other);
        ManageInteractionWithPlatformValue(other);
        ManageInteractionWithValueBonus(other);
        ManageInteractionWithSplitter(other);
        ManageInteractionWithBlackHole(other);
    }

    private void ManageInteractionWithBlackHole(Collision other)
    {
        m_blackHoleBuffer = other.collider.gameObject.GetComponent<BlackHole>();

        if (m_blackHoleBuffer == null)
            return;

        m_body.isKinematic = true;

        m_valueVisual.DOScale(Vector3.one * 0.4f, 1f);
        m_body.transform.DOMove(other.gameObject.transform.position, 1f).OnComplete(Kill);
    }

    private void ManageInteractionWithSplitter(Collision other)
    {
        if (m_value <= 1f)
            return;

        m_splitterBuffer = other.collider.gameObject.GetComponent<Splitter>();

        if (m_splitterBuffer == null)
            return;

        OnHitSplitter?.Invoke(transform.position, m_value);

        m_isInCooldownCollision = true;

        Kill();
    }

    private void ManageInteractionWithValueBonus(Collision other)
    {
        m_valueBonusBuffer = other.collider.gameObject.GetComponent<ValueBonus>();

        if (m_valueBonusBuffer == null)
            return;

        OnHitValueBonus?.Invoke(m_valueBonusBuffer, m_value);

        m_isInCooldownCollision = true;

        m_valueBonusBuffer = null;
    }

    private void ManageInteractionWithPlatformValue(Collision other)
    {
        m_platformValueBuffer = other.collider.gameObject.GetComponentInParent<PlatformValue>();

        if (m_platformValueBuffer == null)
            return;

        OnHitPlatformValue?.Invoke(m_platformValueBuffer, m_value);

        m_isInCooldownCollision = true;

        m_platformValueBuffer = null;

        Kill();
    }

    private void ManageInteractionWithValueBumper(Collision other)
    {
        m_valueBumperBuffer = other.collider.gameObject.GetComponent<ValueBumper>();

        if (m_valueBumperBuffer == null)
            return;

        ApplyBumperEffect(m_valueBumperBuffer.BumpEffect);

        OnHitValueBumper?.Invoke(m_valueBumperBuffer);

        m_isInCooldownCollision = true;

        m_valueBumperBuffer = null;
    }

    #endregion

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
            case ValueBumpEffect.Subtract:
                SetValue(m_value - bumpEffect.BumpValue);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        m_valueVisual.localScale = Vector3.one;

        if (m_tweener != null && m_tweener.IsPlaying())
            return;

        m_tweener = m_valueVisual.DOPunchScale(Vector3.one, 0.33f, 1);
    }

    private void Kill()
    {
        Destroy(gameObject);
    }

    private void OnSpawnValue(ValueActor_Value actorValue, Vector3 ejectionDirection, float value,
        float ejectionStrength)
    {
        if (actorValue != this)
            return;

        m_isInCooldownCollision = true;

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
        if (newValue < 1)
            newValue = 1;
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