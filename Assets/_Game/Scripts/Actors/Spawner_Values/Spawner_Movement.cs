using UnityEngine;

public class Spawner_Movement : MonoBehaviour
{
    [SerializeField] private Vector3 m_originalPosition = Vector3.zero;
    
    [SerializeField] private float m_maxXPosition = 7f;

    private Vector3 m_desiredPosition;
    private Vector3 m_startPosition;
    private float m_progression;

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
        m_startPosition = m_originalPosition;
        m_desiredPosition = m_startPosition;
    }

    private void Update()
    {
        UpdateMovement();
    }

    private void CalculatePosition(Vector3 cursorPosition)
    {
        m_progression = cursorPosition.x / Screen.width;

        m_desiredPosition.x = m_progression * (m_maxXPosition * 2) - m_maxXPosition;

        m_desiredPosition.x = Mathf.Clamp(m_desiredPosition.x, -m_maxXPosition, m_maxXPosition);
    }
    

    private void UpdateMovement()
    {
        transform.position = m_desiredPosition;
    }
    
    
    
}
