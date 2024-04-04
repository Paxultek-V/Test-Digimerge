using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_SpeedZone : MonoBehaviour
{
    [SerializeField] private float m_force = 5f;

    private List<Rigidbody> m_bodyInZoneList = new List<Rigidbody>();

    private Rigidbody m_bodyBuffer;


    private void Update()
    {
        ApplyForceOnBodies();
    }

    private void ApplyForceOnBodies()
    {
        if(m_bodyInZoneList == null || m_bodyInZoneList.Count == 0)
            return;
        
        for (int i = 0; i < m_bodyInZoneList.Count; i++)
        {
            if (m_bodyInZoneList[i] != null)
                m_bodyInZoneList[i].AddForce(transform.up * m_force);
            else
                m_bodyInZoneList.RemoveAt(i);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        m_bodyBuffer = other.gameObject.GetComponent<Rigidbody>();

        if (m_bodyBuffer == null)
            return;

        if (m_bodyInZoneList.Contains(m_bodyBuffer))
            return;

        m_bodyInZoneList.Add(m_bodyBuffer);
    }

    private void OnTriggerExit(Collider other)
    {
        m_bodyBuffer = other.gameObject.GetComponent<Rigidbody>();

        if (m_bodyBuffer == null)
            return;

        if (m_bodyInZoneList.Contains(m_bodyBuffer))
            m_bodyInZoneList.Remove(m_bodyBuffer);
    }
}