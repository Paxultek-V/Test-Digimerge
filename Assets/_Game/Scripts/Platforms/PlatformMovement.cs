using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    [SerializeField] private Transform m_controlledTransform = null;
    
    [SerializeField] private GameObject m_startPosition = null;
    [SerializeField] private GameObject m_endPosition = null;

    [SerializeField] private float m_progressionSpeed = 1f;

    [SerializeField] private float m_gizmosSize = 0.5f;
    
    private float m_progression;
    private float m_timer;


    private void Update()
    {
        UpdatePosition();
    }


    private void UpdatePosition()
    {
        m_timer += Time.deltaTime * m_progressionSpeed;
        m_progression = Mathf.PingPong(m_timer, 1f);

        m_controlledTransform .position = Vector3.Lerp(m_startPosition.transform.position, m_endPosition.transform.position, m_progression);

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        
        Gizmos.DrawSphere(m_startPosition.transform.position, m_gizmosSize);
        Gizmos.DrawSphere(m_endPosition.transform.position, m_gizmosSize);
    }
}
