using System;
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

    private Module_Movement m_movement;


    /// <summary>
    /// called by editor button
    /// </summary>
    public void UpdateMovement()
    {
        RemoveAllMovement();

        switch (m_movementType)
        {
            case MovementType.None:

                break;
            case MovementType.Circle:
                m_movement = GetComponent<Module_Movement_Circle>();

                if (m_movement == null)
                    gameObject.AddComponent<Module_Movement_Circle>();
                break;
            case MovementType.BackForth:
                m_movement = GetComponent<Module_Movement_BackForth>();

                if (m_movement == null)
                    gameObject.AddComponent<Module_Movement_BackForth>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RemoveAllMovement()
    {
        while (gameObject.RemoveComponent<Module_Movement>())
        {
            continue;
        }
    }
}