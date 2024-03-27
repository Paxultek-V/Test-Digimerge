using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChainElement : MonoBehaviour
{
    private MeshCollider m_collider;
    private HingeJoint m_joint;
    private Rigidbody m_body;

    [SerializeField] private float m_minEjectionForce = 0f;
    [SerializeField] private float m_maxEjectionForce = 250f;

    [SerializeField] private float m_maxRotationForce = 250f;

    [SerializeField] private float m_minLifeTime = 0.5f;
    [SerializeField] private float m_maxLifeTime = 1.5f;
    
    private void Awake()
    {
        m_collider = GetComponent<MeshCollider>();
        m_joint = GetComponent<HingeJoint>();
        m_body = GetComponent<Rigidbody>();
    }


    public void DestroyChainElement()
    {
        m_collider.enabled = false;

        Debug.Log("chain destroyed");
        m_joint.connectedBody = null;
        
        m_body.AddForce(Random.insideUnitCircle * Random.Range(m_minEjectionForce, m_maxEjectionForce));

        m_body.AddTorque(Random.insideUnitSphere * Random.Range(-m_maxRotationForce, m_maxRotationForce));
        
        Destroy(gameObject, Random.Range(m_minLifeTime, m_maxLifeTime));
    }

}
