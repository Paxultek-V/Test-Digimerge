using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType
{
    None,
    Circle,
    BackForth
}

public class Element_MovementController : MonoBehaviour
{
    [SerializeField] private MovementType m_movementType;

    private Module_Movement_Circle m_movementCircle;
    private Module_Movement_BackForth m_movementBackForth;
    
    private void Update()
    {
        
    }

    private void ManageMovementModules()
    {
        switch (m_movementType)
        {
            case MovementType.None:
                break;
            case MovementType.Circle:
                m_movementCircle = GetComponent<Module_Movement_Circle>();

                if (m_movementCircle == null)
                    gameObject.AddComponent<Module_Movement_Circle>();
                break;
            case MovementType.BackForth:
                m_movementBackForth = GetComponent<Module_Movement_BackForth>();

                if (m_movementBackForth == null)
                    gameObject.AddComponent<Module_Movement_BackForth>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
