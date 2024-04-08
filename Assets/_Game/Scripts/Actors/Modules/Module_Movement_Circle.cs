using UnityEngine;

public class Module_Movement_Circle : Module_Movement
{
    [SerializeField] private float m_radius = 5f;

    [SerializeField] private float m_angleSpeed = 90f;

    private Vector3 m_startPosition;
    private Vector3 m_desiredPosition;
    private float m_currentAngle;

    private Vector3 m_start;
    private Vector3 m_end;
    private int m_debugResolution = 16;

    private void Awake()
    {
        m_startPosition = transform.position;
    }

    private void Update()
    {
        UpdateMovement();
    }


    private void UpdateMovement()
    {
        transform.position = CalculatePointOnCircle(m_startPosition, m_radius, m_currentAngle);

        m_currentAngle += m_angleSpeed * Time.deltaTime;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        float step = 360f / m_debugResolution;

        for (int i = 0; i < m_debugResolution; i++)
        {
            m_start = CalculatePointOnCircle(m_startPosition, m_radius, step * i);
            m_end = CalculatePointOnCircle(m_startPosition, m_radius, step * (i + 1));
            
            Gizmos.DrawLine(m_start, m_end);
        }
    }

    private Vector3 CalculatePointOnCircle(Vector3 origin, float radius, float angle)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius + origin.x;
        position.y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius + origin.y;
        position.z = 0f;

        return position;
    }
}