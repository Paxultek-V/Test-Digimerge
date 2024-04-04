using UnityEngine;

public class Module_Rotation : MonoBehaviour
{
    [SerializeField] private Transform m_controlledTransform = null;
    
    [SerializeField] private float m_rotationSpeed = 50f;

    private float m_desiredRotation;

    private void Update()
    {
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        m_desiredRotation += m_rotationSpeed * Time.deltaTime;
        m_controlledTransform.Rotate(Vector3.forward, m_rotationSpeed * Time.deltaTime);
    }
}
