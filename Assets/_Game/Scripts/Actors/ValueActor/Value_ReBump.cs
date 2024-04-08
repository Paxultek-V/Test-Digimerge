using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Value_ReBump : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Rigidbody m_body = null;

    [SerializeField] private float m_reBumpForce = 10f;
    [SerializeField] private float m_minDistanceToReBump = 0.01f;
    [SerializeField] private float m_delayBeforeReBump = 2f;
    [SerializeField] private float m_reBumpCooldown = 1f;

    private Vector3 m_previousPosition;
    private Vector3 m_reBumpDirection;
    private float m_reBumpTimer;
    private float m_cooldownTimer;
    private bool m_isInCooldown;

    private void Start()
    {
        m_previousPosition = m_body.position;
        StartCoroutine(CheckReBumpConditionCoroutine());
    }

    private void Update()
    {
        CheckReBumpCondition();
        ManageCooldown();
    }

    private void CheckReBumpCondition()
    {
        if (m_isInCooldown)
            return;

        if (Vector3.Distance(m_body.position, m_previousPosition) < m_minDistanceToReBump)
        {
            m_reBumpTimer += Time.deltaTime;

            if (m_reBumpTimer > m_delayBeforeReBump)
                ReBump();
        }
        else
        {
            m_reBumpTimer = 0f;
        }
    }

    private IEnumerator CheckReBumpConditionCoroutine()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1f);

            m_previousPosition = m_body.position;
        }
    }

    private void ReBump()
    {
        m_reBumpDirection = Random.insideUnitSphere;
        m_reBumpDirection.z = 0f;
        m_reBumpDirection.y = Mathf.Abs(m_reBumpDirection.y);
        m_reBumpDirection.Normalize();

        m_body.AddForce(m_reBumpDirection * m_reBumpForce);
        Debug.Log("BUMP");
        EnterCooldown();
    }

    private void EnterCooldown()
    {
        m_isInCooldown = true;
        m_cooldownTimer = 0f;
    }

    private void ManageCooldown()
    {
        if (!m_isInCooldown)
            return;

        m_cooldownTimer += Time.deltaTime;

        if (m_cooldownTimer > m_reBumpCooldown)
        {
            m_isInCooldown = false;
        }
    }
}