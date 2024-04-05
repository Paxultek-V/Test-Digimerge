using UnityEngine;

public class Spawner_Movement : MonoBehaviour
{
    [SerializeField] private Spawner_Value_SO m_spawnerData = null;

    public Vector3 m_desiredPosition;
    public Vector3 m_startPosition;
    public Vector3 m_startCursorPosition;
    public Vector3 m_currentCursorPosition;
    private Vector3 m_progressionPosition;
    private float m_progression;
    private float m_velocity;

    private void OnEnable()
    {
        Controller.OnTapBegin += OnTapBegin;
        Controller.OnHold += OnHold;
        Controller.OnRelease += OnRelease;
    }

    private void OnDisable()
    {
        Controller.OnTapBegin -= OnTapBegin;
        Controller.OnHold -= OnHold;
        Controller.OnRelease -= OnRelease;
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
        if(UI_Debug.IsDebugPanelOpen)
            return;
        
        UpdateMovement();
    }

    private void OnTapBegin(Vector3 cursorPosition)
    {
        m_startPosition = transform.position;
        m_startCursorPosition = cursorPosition;
    }

    private void OnHold(Vector3 cursorPosition)
    {
        m_currentCursorPosition = cursorPosition;

        Vector3 cursorPositionDiff = m_currentCursorPosition - m_startCursorPosition;

        m_desiredPosition = m_startPosition;
        m_desiredPosition.x = m_startPosition.x + cursorPositionDiff.x * (1f/m_spawnerData.pixelPerMeter);
        
        m_desiredPosition.x = Mathf.Clamp(m_desiredPosition.x, -m_spawnerData.maxXPosition, m_spawnerData.maxXPosition);
    }

    private void OnRelease(Vector3 cursorPosition)
    {
        m_startPosition = transform.position;
        m_startCursorPosition = cursorPosition;
        m_currentCursorPosition = cursorPosition;
        m_desiredPosition = transform.position;
    }


    private void UpdateMovement()
    {
        m_progressionPosition.x = Mathf.SmoothDamp(m_progressionPosition.x, m_desiredPosition.x, ref m_velocity,
            m_spawnerData.smoothTime);
        transform.position = m_progressionPosition;
    }
}