using UnityEngine;

public class Spawner_Movement_Auto : MonoBehaviour
{
    [SerializeField] private Spawner_Value_SO m_spawnerData = null;

    public Vector3 m_desiredPosition;
    public Vector3 m_startPosition;
    private Vector3 m_progressionPosition;
    private float m_progression;
    private float m_velocity;
    
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        m_startPosition = transform.position;
        m_desiredPosition = m_startPosition;
        m_progressionPosition = m_startPosition;
    }

    private void Update()
    {
        if(UI_Debug.IsDebugPanelOpen)
            return;
        DecideDesiredPosition();
        UpdateMovement();
    }
    
    private void DecideDesiredPosition()
    {
        m_desiredPosition = m_startPosition;
        m_desiredPosition.x = m_startPosition.x + Mathf.Sin(Time.time * m_spawnerData.autoMovementSpeed) * m_spawnerData.maxXPosition;
        m_desiredPosition.x = Mathf.Clamp(m_desiredPosition.x, -m_spawnerData.maxXPosition, m_spawnerData.maxXPosition);
    }

    
    private void UpdateMovement()
    {
        m_progressionPosition.x = Mathf.SmoothDamp(m_progressionPosition.x, m_desiredPosition.x, ref m_velocity,
            m_spawnerData.smoothTime);
        
        transform.position = m_progressionPosition;
    }
}