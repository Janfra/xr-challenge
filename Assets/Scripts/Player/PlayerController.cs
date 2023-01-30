using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Action<bool, PlayerInputs> OnUpdateInputs;
    private bool isEnabled = true;

    #region Dependencies

    [Header("Dependencies")]
    [SerializeField]
    private Rigidbody playerRigidbody;
    public Rigidbody PlayerRigidbody => playerRigidbody;
    private PlayerInputs actionInputs;
    public PlayerInputs ActionInputs => actionInputs;

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
        GameManager.OnSetInputs += SetInputs;
        GameManager.Instance.RequestInputs();

        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }

        GameManager.OnGameStateChanged += OnPause;
        GameManager.OnGameStateChanged += UpdateComponents;
    }

    private void UpdateComponents(GameManager.GameStates _newState)
    {
        if(_newState != GameManager.GameStates.Main)
        {
            jumpHandler.OnDestroy(actionInputs);
            movementHandler.OnDestroy(actionInputs);
            rotationHandler.OnDestroy(actionInputs);
        } 
        else if (_newState == GameManager.GameStates.Main)
        {
            movementHandler.Init(this);
            rotationHandler.Init(this);
            jumpHandler.Init(this);
        }
    }

    private void Start()
    {
        movementHandler.Init(this);
        rotationHandler.Init(this);
        jumpHandler.Init(this);
        Dialogue.OnDialogue += SetControllersEnabled;
    }

    private void SetInputs(PlayerInputs _inputs)
    {
        actionInputs = _inputs;
    }

    /// <summary>
    /// Resets all components and events
    /// </summary>
    private void OnEnable()
    {
        GameManager.OnSetInputs += SetInputs;
        GameManager.Instance.RequestInputs();
        EnableControllers();
        actionInputs.Player.Interact.Enable();
        GameManager.OnDeviceUpdate += IsGamepadControlSchemeUsed;
        GameManager.OnGameStateChanged += OnPause;

        playerRigidbody = GetComponent<Rigidbody>();
        movementHandler.Init(this);
        rotationHandler.Init(this);
        jumpHandler.Init(this);
    }
    
    /// <summary>
    /// Disables all components and events
    /// </summary>
    private void OnDisable()
    {
        DisableControllers();
        actionInputs.Player.Interact.Disable();
        GameManager.OnDeviceUpdate -= IsGamepadControlSchemeUsed;
        GameManager.OnGameStateChanged -= OnPause;
        jumpHandler.OnDestroy(actionInputs);
        movementHandler.OnDestroy(actionInputs);
        rotationHandler.OnDestroy(actionInputs);
    }

    /// <summary>
    /// Disables all components and events
    /// </summary>
    private void OnDestroy()
    {
        DisableControllers();
        actionInputs.Player.Interact.Disable();
        jumpHandler.OnDestroy(actionInputs);
        movementHandler.OnDestroy(actionInputs);
        rotationHandler.OnDestroy(actionInputs);
        Dialogue.OnDialogue -= SetControllersEnabled;
        GameManager.OnGameStateChanged -= OnPause;
        GameManager.OnSetInputs -= SetInputs;
        GameManager.OnGameStateChanged -= UpdateComponents;
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
            rotationHandler.GetRotation();
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
    private void OnPause(GameManager.GameStates _newState)
    {
        if (_newState == GameManager.GameStates.Pause)
        {
            SetControllersEnabled(false);
            playerRigidbody.useGravity = false;
            playerRigidbody.Sleep();
        }
        else
        {
            SetControllersEnabled(true);
            playerRigidbody.useGravity = true;
            playerRigidbody.WakeUp();
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
        if (actionInputs != null)
        {
            Debug.Log("Disabled player controllers");
            actionInputs.Player.Move.Disable();
            actionInputs.Player.Jumping.Disable();
            actionInputs.Player.Look.Disable();
        }
        else
        {
            Debug.LogWarning($"Player has no action inputs on destroy!");
        }
    }

    /// <summary>
    /// Enables movement controllers 
    /// </summary>
    private void EnableControllers()
    {
        if(actionInputs != null)
        {
            Debug.Log("Enabled player controllers");
            actionInputs.Player.Move.Enable();
            actionInputs.Player.Jumping.Enable();
            actionInputs.Player.Look.Enable();
        }
        else
        {
            GameManager.Instance.RequestInputs();
        }
    }
    
    /// <summary>
    /// Updates inputs to use gamepad or keyboard/mouse controllers 
    /// </summary>
    /// <param name="_isGamepad">True if gamepad is currently being used</param>
    private void IsGamepadControlSchemeUsed(bool _isGamepad)
    {
        OnUpdateInputs(_isGamepad, actionInputs);
    }

    private void OnDrawGizmos()
    {
        jumpHandler.OnGizmos();

        movementHandler.OnGizmos();

        rotationHandler.OnGizmos();
    }
}
