using System;
using System.Collections;
using UnityEngine;

public class Spawner_SpawningCondition : MonoBehaviour
{
    public static Action<float> OnSendAdditionalTimeBeforeSpawnStopsTimer;
    public Action OnAdditionalTimeBeforeSpawnStopsOver;
    
    [SerializeField] private Spawner_Value_SO m_spawnerData = null;
    
    private bool m_isInCooldown; //flag to manage the spawning speed
    private bool m_isPlayerTouchingScreen; //flag to manage spawning  with the input controls
    private bool m_canSpawnInLevel; //flag to manage the overall state of spawning

    private float m_cooldownTimer;
    private float m_cooldownDuration;
    private float m_additionalTimeBeforeSpawnStopsTimer;
    
    public bool CanSpawn
    {
        get => (!m_isInCooldown && m_isPlayerTouchingScreen && m_canSpawnInLevel);
    }
    
    private void OnEnable()
    {
        Controller.OnTapBegin += StartSpawning;
        Controller.OnRelease += StopSpawning;

        Controller_LevelSection.OnNextLevelSectionLoaded += OnNextLevelSectionLoaded;
        Controller_LevelSection.OnStartLoadingNextSectionLevel += OnStartLoadingNextSectionLevel;
        
        PiggyBank.OnTargetAmountReached += OnTargetAmountReached;
    }

    private void OnDisable()
    {
        Controller.OnTapBegin -= StartSpawning;
        Controller.OnRelease -= StopSpawning;

        Controller_LevelSection.OnNextLevelSectionLoaded -= OnNextLevelSectionLoaded;
        Controller_LevelSection.OnStartLoadingNextSectionLevel -= OnStartLoadingNextSectionLevel;
        
        PiggyBank.OnTargetAmountReached -= OnTargetAmountReached;
    }

    private void Start()
    {
        /*m_isInCooldown = false;
        EnableSpawnInLevel();*/
    }

    private void Update()
    {
        ManageCooldown();
    }

    private void ManageCooldown()
    {
        if(!m_isInCooldown)
            return;

        m_cooldownTimer += Time.deltaTime;

        if (m_cooldownTimer >= m_cooldownDuration)
            m_isInCooldown = false;
    }

    private void OnNextLevelSectionLoaded()
    {
        EnableSpawnInLevel();
    }

    private void OnStartLoadingNextSectionLevel()
    {
        DisableSpawnInLevel();
    }
    
    private void OnTargetAmountReached()
    {
        StartCoroutine(AdditionalTimeBeforeSpawnStops());
    }

    private IEnumerator AdditionalTimeBeforeSpawnStops()
    {
        m_additionalTimeBeforeSpawnStopsTimer = 0f;

        while (m_additionalTimeBeforeSpawnStopsTimer < m_spawnerData.additionalTimeBeforeSpawnStops)
        {
            m_additionalTimeBeforeSpawnStopsTimer += Time.unscaledDeltaTime;

            OnSendAdditionalTimeBeforeSpawnStopsTimer?.Invoke(m_spawnerData.additionalTimeBeforeSpawnStops -
                                                              m_additionalTimeBeforeSpawnStopsTimer);

            yield return new WaitForEndOfFrame();
        }

        DisableSpawnInLevel();
        
        OnAdditionalTimeBeforeSpawnStopsOver?.Invoke();
    }
    
    
    public void EnterCooldown(float cooldownDuration)
    {
        m_isInCooldown = true;
        m_cooldownTimer = 0f;
        m_cooldownDuration = cooldownDuration;
    }
    
    public void EnableSpawnInLevel()
    {
        m_canSpawnInLevel = true;
    }

    public void DisableSpawnInLevel()
    {
        m_canSpawnInLevel = false;
    }

    private void StartSpawning(Vector3 cursorPosition)
    {
        m_isPlayerTouchingScreen = true;
    }

    private void StopSpawning(Vector3 cursorPosition)
    {
        m_isPlayerTouchingScreen = false;
    }
}
