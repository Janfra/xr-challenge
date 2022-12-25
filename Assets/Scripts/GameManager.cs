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

    [Header("Dependencies")]
    [SerializeField]
    private ScenesAvailable[] scenesAvailable;
    private Dictionary<GameStates, string> scenesForState;

    private void Awake()
    {
        // Singleton
        if(Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;

            currentState = GameStates.Start;

            scenesForState = new Dictionary<GameStates, string>();
            foreach (var scene in scenesAvailable)
            {
                if (!scenesForState.ContainsKey(scene.sceneState))
                {
                    scenesForState.Add(scene.sceneState, scene.sceneName);
                }
            }
        }
        else
        {
            Destroy(this);
        }

    }
    
    public void UpdateState(GameStates _gameState)
    {
        GameStates pastState = currentState;
        currentState = _gameState;

        switch (currentState)
        {
            case GameStates.Start:
                break;
            case GameStates.Pause:
                break;
            case GameStates.Main:
                if(pastState != GameStates.Main && pastState != GameStates.Pause)
                {
                    TryLoadScene(currentState);
                }
                break;
            case GameStates.End:
                break;
            default:
                break;
        }

        OnGameStateChanged?.Invoke(currentState);
    }

    private void TryLoadScene(GameStates _gameState)
    {
        if (scenesForState.TryGetValue(_gameState, out string stateSceneName))
        {
            SceneManager.LoadScene(stateSceneName);
        }
        else
        {
            Debug.LogError("State scene not found!");
        }
    }

    public void OnStartGame()
    {
        Debug.Log("Started Game");
        UpdateState(GameStates.Main);
    }

    public enum GameStates
    {
        Start,
        Pause,
        Main,
        End,
    }

    [System.Serializable]
    private struct ScenesAvailable
    {
        public GameStates sceneState;
        public string sceneName;
    }
}
