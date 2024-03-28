using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Controller_Gravity : MonoBehaviour
{
    [SerializeField] private float m_desiredGravityStrength = -5f;
    
    
    void Start()
    {
        Physics.gravity = new Vector3(0f, m_desiredGravityStrength, 0f); 
    }
}
