using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static event Action<GameStates> OnGameStateChanged;

    /// <summary>
    /// Called when the game is now using generally relevant inputs 
    /// </summary>
    public static event Action<PlayerInputs> OnSetInputs;

    /// <summary>
    /// Called when there is a change in input device. 
    /// Bool is for is gamepad being used
    /// </summary>
    public static event Action<bool> OnDeviceUpdate;
    public static GameManager Instance;

    private PlayerInputs inputs;

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

            inputs = new PlayerInputs();
            inputs.UI.Enable();
            inputs.UI.Pause.started += context =>
            {
                UpdateState(GameStates.Pause);
            };
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

        if (inputs != null)
        {
            inputs.UI.Disable();
            inputs.UI.Pause.started -= context =>
            {
                UpdateState(GameStates.Pause);
            };
        }
    }

    #region Buttons Functions

    /// <summary>
    /// Quits the application
    /// </summary>
    public static void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    /// <summary>
    /// Sets the game state to main
    /// </summary>
    public static void PlayAgain()
    {
        Debug.Log("PlayAgain");
        if (Instance != null)
        {
            Instance.UpdateState(GameStates.Main);
        }
        else
        {
            Debug.LogError("No instance set");
        }
    }

    /// <summary>
    /// Sets the game state to start
    /// </summary>
    public static void BackToMainMenu()
    {
        Debug.Log("BackToMenu");
        if (Instance != null)
        {
            Instance.UpdateState(GameStates.Start);
        }
        else
        {
            Debug.LogError("No instance set");
        }
    }

    /// <summary>
    /// Sets the game state to main 
    /// </summary>
    public static void ResumeGame()
    {
        if (Instance != null)
        {
            Instance.UpdateState(GameStates.Main);
        }
        else
        {
            Debug.LogError("No instance set on game manager");
        }
    }

    /// <summary>
    /// Sets the game state to main
    /// </summary>
    public static void OnStartGame()
    {
        if (Instance != null)
        {
            Debug.Log("Started Game");
            Instance.UpdateState(GameStates.Main);
        }
    }

    #endregion

    #region Game States

    /// <summary>
    /// Updates the game state to the given state and runs any basic logic
    /// </summary>
    /// <param name="_gameState">New game state</param>
    public void UpdateState(GameStates _gameState)
    {
        GameStates pastState = currentState;
        currentState = _gameState;

        switch (currentState)
        {
            case GameStates.Start:
                inputs.UI.Pause.Disable();
                if(pastState != GameStates.Start)
                {
                    TryLoadScene(currentState);
                    Button selectedButton = ButtonUI.OnGetSelectedButton();
                    if (selectedButton)
                    {
                        selectedButton.Select();
                    }
                }

                break;
            case GameStates.Pause:
                if(pastState == GameStates.Pause)
                {
                    Unpaused();
                }
                else if(currentState == GameStates.Pause)
                {
                    Button selectedButton = ButtonUI.OnGetSelectedButton();
                    if (selectedButton)
                    {
                        selectedButton.Select();
                    }
                }

                break;
            case GameStates.Main:
                inputs.UI.Pause.Enable();
                if(pastState != GameStates.Main && pastState != GameStates.Pause)
                {
                    TryLoadScene(currentState);
                }

                break;
            case GameStates.End:
                inputs.UI.Pause.Disable();
                if(pastState != GameStates.End)
                {
                    TryLoadScene(currentState);
                }

                break;
            default:
                Debug.LogError("State not set in update state");

                break;
        }

        OnGameStateChanged?.Invoke(currentState);
    }

    /// <summary>
    /// Calls event to send inputs to event listeners, only available at beginning of scene
    /// </summary>
    public void RequestInputs()
    {
        if (Time.timeSinceLevelLoad < Time.time + 1f)
        {
            OnSetInputs?.Invoke(inputs);
            Debug.Log($"Inputs requested and sent");
        }
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

    private void Unpaused()
    {
        currentState = GameStates.Main;
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