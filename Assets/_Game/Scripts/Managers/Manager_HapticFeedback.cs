using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Manager_HapticFeedback : MonoBehaviour
{
    private bool m_isHapticEnabled;
    
    private void OnEnable()
    {
        Taptic.tapticOn = true;

        Manager_GameState.OnBroadcastGameState += OnBroadcastGameState;
        
        Spawner_ValueActor.OnSpawnValue += OnSpawnValue;
        ValueActor_Value.OnHitValueBumper += OnHitValueBumper;
    }

    private void OnDisable()
    {
        Taptic.tapticOn = false;
        
        Manager_GameState.OnBroadcastGameState -= OnBroadcastGameState;
        
        Spawner_ValueActor.OnSpawnValue -= OnSpawnValue;
        ValueActor_Value.OnHitValueBumper -= OnHitValueBumper;
    }


    public void EnableHaptic()
    {
        m_isHapticEnabled = true;
    }

    public void DisableHaptic()
    {
        m_isHapticEnabled = false;
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
        if(!m_isHapticEnabled)
            return;
        
        Taptic.Light();
    }

    private void PlayHeavyHaptic()
    {
        if(!m_isHapticEnabled)
            return;
        
        Taptic.Heavy();
    }

}
