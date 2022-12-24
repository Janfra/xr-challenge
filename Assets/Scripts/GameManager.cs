using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static event Action<GameStates> OnGameStateChanged;
    public static GameManager Instance;

    private GameStates currentState;
    public GameStates CurrentState => currentState;

    private void Awake()
    {
        // Singleton
        if(Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    public void UpdateState(GameStates _gameState)
    {
        currentState = _gameState;

        switch (currentState)
        {
            case GameStates.Start:
                break;
            case GameStates.Main:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;
            case GameStates.End:
                break;
            default:
                break;
        }

        OnGameStateChanged?.Invoke(currentState);
    }

    public void OnStartGame()
    {
        Debug.Log("Started Game");
        UpdateState(GameStates.Main);
    }

    public enum GameStates
    {
        Start,
        Main,
        End,
    }
}
