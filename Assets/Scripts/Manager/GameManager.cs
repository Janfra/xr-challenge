using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static event Action<GameStates> OnGameStateChanged;
    /// <summary>
    /// Called when there is a change in input device. 
    /// Bool is for is gamepad being used
    /// </summary>
    public static event Action<bool> OnDeviceUpdate;
    public static GameManager Instance;

    #region GameStates

    private GameStates currentState;
    public GameStates CurrentState => currentState;

    [Header("Dependencies")]
    [SerializeField]
    private ScenesAvailable[] scenesAvailable;
    private Dictionary<GameStates, string> scenesForState;

    #endregion

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

    private void Start()
    {
        InputSystem.onDeviceChange += OnDeviceChanged;
        InputUser.onChange += OnControlSchemeChanged;
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChanged;
        InputUser.onChange -= OnControlSchemeChanged;
    }

    #region Game States
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

    #endregion

    #region Device Handling

    /// <summary>
    /// Updates based on device change
    /// </summary>
    /// <param name="_device"></param>
    /// <param name="_deviceChange"></param>
    private void OnDeviceChanged(InputDevice _device, InputDeviceChange _deviceChange)
    {
        // Log info
        const int MAX_STRING_SIZE = 10;
        string deviceName = _device.ToString().Substring(0, MAX_STRING_SIZE);
        Debug.Log($"Device {_deviceChange}: {deviceName}, Device type: {_device.displayName}");

        // Is gamepad available now
        bool isGamepadAvailable = Gamepad.current != null;

        switch (_deviceChange)
        {
            case InputDeviceChange.Added:
            case InputDeviceChange.Reconnected:
                if (isGamepadAvailable && IsConnectedDeviceGamepad(_device))
                {
                    OnDeviceUpdate?.Invoke(isGamepadAvailable);
                    Debug.Log("Gamepad available, setting controllers to gamepad!");
                }
                break;

            case InputDeviceChange.Removed:
            case InputDeviceChange.Disconnected:
                OnDeviceUpdate?.Invoke(isGamepadAvailable);
                if (!isGamepadAvailable)
                {
                    Debug.Log("No gamepad controllers available, switching to keyboard controllers!");
                }
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Check if device given is in the gamepad list
    /// </summary>
    /// <param name="_device">Deviced being checked</param>
    /// <returns></returns>
    private bool IsConnectedDeviceGamepad(InputDevice _device)
    {
        bool isConnectedDeviceGamepad = false;
        Gamepad isGamepad = (Gamepad)_device;

        if (isGamepad != null)
        {
            foreach (Gamepad gamepad in Gamepad.all)
            {
                if (gamepad == isGamepad)
                {
                    isConnectedDeviceGamepad = true;
                    break;
                }
            }
        }
        else
        {
            Debug.Log("Connected device is not a gamepad");
        }

        return isConnectedDeviceGamepad;
    }

    /// <summary>
    /// Calls event to update current input device type used
    /// </summary>
    /// <param name="_inputUser">Current control scheme used</param>
    /// <param name="_inputChange">Type of input change</param>
    private void OnControlSchemeChanged(InputUser _inputUser, InputUserChange _inputChange, InputDevice _device)
    {
        switch (_inputChange)
        {
            case InputUserChange.ControlSchemeChanged:
                // Get scheme name and check if it is gamepad
                string currentSchemeName = _inputUser.controlScheme.Value.name;
                bool isGamepad = currentSchemeName == "Gamepad";

                // Update controller type
                OnDeviceUpdate?.Invoke(isGamepad);
                Debug.Log($"Current Scheme is: {currentSchemeName}");
                break;

            default:
                break;
        }
    }

    #endregion
}