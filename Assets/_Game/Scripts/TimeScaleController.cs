using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
    [SerializeField] private float m_desiredTimeScale = 1f;
    
    
    void Start()
    {
        Time.timeScale = m_desiredTimeScale;
    }

}
