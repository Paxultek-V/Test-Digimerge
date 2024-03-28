using TMPro;
using UnityEngine;

public class UI_AdditionalTimeBeforeSpawnStops : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_timerText = null;


    private void OnEnable()
    {
        Spawner_SpawningCondition.OnSendAdditionalTimeBeforeSpawnStopsTimer += OnSendAdditionalTimeBeforeSpawnStopsTimer;
    }

    private void OnDisable()
    {
        Spawner_SpawningCondition.OnSendAdditionalTimeBeforeSpawnStopsTimer -= OnSendAdditionalTimeBeforeSpawnStopsTimer;
    }

    private void Start()
    {
        m_timerText.text = "";
    }

    private void OnSendAdditionalTimeBeforeSpawnStopsTimer(float time)
    {
        m_timerText.text = time.ToString("F0");

        if (time <= 0)
            m_timerText.text = "";
    }
}