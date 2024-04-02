using UnityEngine;

public class Spawner_Movement : MonoBehaviour
{
    [SerializeField] private Spawner_Value_SO m_spawnerData = null;

    private Vector3 m_desiredPosition;
    private Vector3 m_startPosition;
    private Vector3 m_progressionPosition;
    private float m_progression;
    private float m_velocity;

    private void OnEnable()
    {
        Controller.OnHold += CalculatePosition;
    }

    private void OnDisable()
    {
        Controller.OnHold -= CalculatePosition;
    }

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
        UpdateMovement();
    }

    
    private void CalculatePosition(Vector3 cursorPosition)
    {
        m_progression = cursorPosition.x / Screen.width;

        m_desiredPosition.x = m_progression * (m_spawnerData.maxXPosition * 2) - m_spawnerData.maxXPosition;

        m_desiredPosition.x = Mathf.Clamp(m_desiredPosition.x, -m_spawnerData.maxXPosition, m_spawnerData.maxXPosition);
    }


    private void UpdateMovement()
    {
        m_progressionPosition.x = Mathf.SmoothDamp(m_progressionPosition.x, m_desiredPosition.x, ref m_velocity,
            m_spawnerData.smoothTime);
        transform.position = m_progressionPosition;
    }
}