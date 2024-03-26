using System;
using UnityEngine;

public enum GameState
{
    MainMenu,
    InGame,
    Gameover,
    Victory
}

public class Manager_GameState : MonoBehaviour
{
    public static Action<GameState> OnBroadcastGameState;

    private GameState m_currentGameState;


    private void OnEnable()
    {
        Button_StartGame.OnStartGameButtonPressed += OnStartGameButtonPressed;
        Controller_LevelSection.OnFinishedLevel += OnFinishedLevel;
        PiggyBank.OnTargetAmountNotReached += OnTargetAmountNotReached;
    }

    private void OnDisable()
    {
        Button_StartGame.OnStartGameButtonPressed -= OnStartGameButtonPressed;
        Controller_LevelSection.OnFinishedLevel -= OnFinishedLevel;
        PiggyBank.OnTargetAmountNotReached -= OnTargetAmountNotReached;
    }


    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        ToMainMenu();
    }

    public void ToMainMenu()
    {
        m_currentGameState = GameState.MainMenu;
        BroadcastGameState();
    }
    
    public void StartGame()
    {
        m_currentGameState = GameState.InGame;
        BroadcastGameState();
    }

    private void OnFinishedLevel()
    {
        m_currentGameState = GameState.Victory;
        BroadcastGameState();
    }

    private void OnTargetAmountNotReached()
    {
        m_currentGameState = GameState.Gameover;
        BroadcastGameState();
    }

    private void OnStartGameButtonPressed()
    {
        StartGame();
    }
    
    private void BroadcastGameState()
    {
        OnBroadcastGameState?.Invoke(m_currentGameState);
    }
}
