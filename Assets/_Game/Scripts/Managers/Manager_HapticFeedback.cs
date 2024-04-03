using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Manager_HapticFeedback : MonoBehaviour
{
    public static Action<bool> OnToggleHaptic;

    private bool m_isHapticEnabled;

    private void OnEnable()
    {
        Taptic.tapticOn = true;

        Manager_GameState.OnBroadcastGameState += OnBroadcastGameState;

        Spawner_ValueActor.OnSpawnValue += OnSpawnValue;
        ValueActor_Value.OnHitValueBumper += OnHitValueBumper;

        Button_ToggleHaptic.OnToggleHaptic_ButtonPressed += OnToggleHaptic_ButtonPressed;
    }

    private void OnDisable()
    {
        Taptic.tapticOn = false;

        Manager_GameState.OnBroadcastGameState -= OnBroadcastGameState;

        Spawner_ValueActor.OnSpawnValue -= OnSpawnValue;
        ValueActor_Value.OnHitValueBumper -= OnHitValueBumper;
        
        Button_ToggleHaptic.OnToggleHaptic_ButtonPressed -= OnToggleHaptic_ButtonPressed;
    }

    private void Start()
    {
        m_isHapticEnabled = true;
        OnToggleHaptic?.Invoke(m_isHapticEnabled);
    }

    private void OnToggleHaptic_ButtonPressed()
    {
        ToggleHaptic(!m_isHapticEnabled);
    }

    public void ToggleHaptic(bool state)
    {
        m_isHapticEnabled = state;
        OnToggleHaptic?.Invoke(m_isHapticEnabled);
    }


    private void OnBroadcastGameState(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                break;
            case GameState.InGame:
                break;
            case GameState.Gameover:
                PlayHeavyHaptic();
                break;
            case GameState.Victory:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }


    private void OnSpawnValue(ValueActor_Value value, Vector3 pos, float a, float b)
    {
        PlayLightHaptic();
    }

    private void OnHitValueBumper(ValueBumper valueBumper)
    {
        PlayLightHaptic();
    }

    private void PlayLightHaptic()
    {
        if (!m_isHapticEnabled)
            return;

        Taptic.Light();
    }

    private void PlayHeavyHaptic()
    {
        if (!m_isHapticEnabled)
            return;

        Taptic.Heavy();
    }
}