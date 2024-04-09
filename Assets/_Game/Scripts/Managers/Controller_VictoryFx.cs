using System.Collections.Generic;
using UnityEngine;

public class Controller_VictoryFx : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> m_victoryFxList = null;


    private void OnEnable()
    {
        Manager_GameState.OnBroadcastGameState += OnBroadcastGameState;
    }

    private void OnDisable()
    {
        Manager_GameState.OnBroadcastGameState -= OnBroadcastGameState;
    }


    private void OnBroadcastGameState(GameState state)
    {
        if (state == GameState.Victory)
            PlayVictoryFx();
    }

    private void PlayVictoryFx()
    {
        for (int i = 0; i < m_victoryFxList.Count; i++)
        {
            m_victoryFxList[i].Play();
        }
    }
    
}
