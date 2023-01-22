using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Called when there is a change in input device. 
    /// Bool is for is gamepad being used
    /// </summary>
    public Action<bool, PlayerInputs> OnDeviceUpdate;
    private bool isEnabled = true;

    #region Dependencies

    [Header("Dependencies")]
    [SerializeField]
    private Rigidbody playerRigidbody;
    private PlayerInputs playerInputs;
    public PlayerInputs PlayerInputs => playerInputs;

    #endregion
     
    #region Components

    [Header("Components")]
    [SerializeField]
    private PlayerRotation rotationHandler;
    [SerializeField]
    private PlayerJump jumpHandler;
    public PlayerJump JumpHandler => jumpHandler;

    [SerializeField]
    private PlayerMovement movementHandler;

    #endregion

    private void Awake()
    {
        playerInputs = new PlayerInputs();
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }

        playerInputs.UI.Pause.started += context => 
        {
            Debug.Log("Paused");
            PauseHandler();
        };
    }

    private void Start()
    {
        movementHandler.Init(this);
        rotationHandler.Init(this);
        jumpHandler.Init(playerInputs);

        Dialogues.OnDialogue += SetControllersEnabled; 
    }

    private void OnEnable()
    {
        EnableControllers();
        playerInputs.Player.Interact.Enable();
        playerInputs.UI.Enable();
        InputSystem.onDeviceChange += OnDeviceChanged;
        InputUser.onChange += OnControlSchemeChanged;
    }

    private void OnDisable()
    {
        DisableControllers();
        playerInputs.Player.Interact.Disable();
        playerInputs.UI.Disable();
        InputSystem.onDeviceChange -= OnDeviceChanged;
        InputUser.onChange -= OnControlSchemeChanged;
    }

    private void OnDestroy()
    {
        playerInputs.UI.Pause.started -= context => PauseHandler();
        jumpHandler.OnDestroy(playerInputs);
        movementHandler.OnDestroy(playerInputs);
        rotationHandler.OnDestroy(playerInputs);
        Dialogues.OnDialogue -= SetControllersEnabled;
    }

    /// <summary>
    /// Updates variables for logic
    /// </summary>
    private void Update()
    {
        if(isEnabled)
        {
            jumpHandler.OnUpdate();
            movementHandler.GetInputs();
            rotationHandler.GetRotation(transform);
        }
    }

    /// <summary>
    /// Runs physics base logic
    /// </summary>
    private void FixedUpdate()
    {
        if (isEnabled)
        {
            jumpHandler.HandleJump();
            movementHandler.HandleMovement();
        }
    }

    /// <summary>
    /// Stops player input and all movement
    /// </summary>
    private void PauseHandler()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameStates.Pause)
        {
            SetControllersEnabled(false);
            playerRigidbody.useGravity = false;
            playerRigidbody.Sleep();
            GameManager.Instance.UpdateState(GameManager.GameStates.Pause);
        }
        else
        {
            SetControllersEnabled(true);
            playerRigidbody.useGravity = true;
            playerRigidbody.WakeUp();
            GameManager.Instance.UpdateState(GameManager.GameStates.Main);
        }
    }

    /// <summary>
    /// Sets whether the player controller should be working
    /// </summary>
    /// <param name="_isEnabled">Is controller active</param>
    public void SetControllersEnabled(bool _isEnabled)
    {
        Debug.Log($"Set player controller to be {_isEnabled}");
        isEnabled = _isEnabled;
        if (isEnabled)
        {
            EnableControllers();
        }
        else
        {
            DisableControllers();
        }
    }

    /// <summary>
    /// Enables player to jump again for the jump buffer duration
    /// </summary>
    public void EnableJumping()
    {
        jumpHandler.ResetCoyoteTime();
    }

    /// <summary>
    /// Disables movement controllers
    /// </summary>
    private void DisableControllers()
    {
        Debug.Log("Disabled player controllers");
        playerInputs.Player.Move.Disable();
        playerInputs.Player.Jumping.Disable();
        playerInputs.Player.Look.Disable();
    }

    /// <summary>
    /// Enables movement controllers 
    /// </summary>
    private void EnableControllers()
    {
        Debug.Log("Enabled player controllers");
        playerInputs.Player.Move.Enable();
        playerInputs.Player.Jumping.Enable();
        playerInputs.Player.Look.Enable();
    }

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
                    OnDeviceUpdate?.Invoke(isGamepadAvailable, playerInputs);
                    Debug.Log("Gamepad available, setting controllers to gamepad!");
                }
                break;

            case InputDeviceChange.Removed:
            case InputDeviceChange.Disconnected:
                OnDeviceUpdate?.Invoke(isGamepadAvailable, playerInputs);
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
        Debug.Log("Input Change");
        switch (_inputChange)
        {
            case InputUserChange.ControlSchemeChanged:
                // Get scheme name and check if it is gamepad
                string currentSchemeName = _inputUser.controlScheme.Value.name;
                bool isGamepad = currentSchemeName == "Gamepad";

                // Update controller type
                OnDeviceUpdate?.Invoke(isGamepad, playerInputs);
                Debug.Log($"Current Scheme is: {currentSchemeName}");
                break;

            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        jumpHandler.OnGizmos();

        movementHandler.OnGizmos();
    }
}
