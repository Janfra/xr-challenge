using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isEnabled = true;

    [Header("Dependencies")]
    [SerializeField]
    private Rigidbody playerRigidbody;
    private PlayerInputs playerInputs;
    public PlayerInputs PlayerInputs => playerInputs;
     
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

        playerInputs.UI.Pause.started += context => PauseHandler();
    }

    private void Start()
    {
        movementHandler.Init(transform, playerInputs);
        rotationHandler.Init(playerInputs);
        jumpHandler.Init(playerInputs);

        Dialogues.OnDialogue += SetEnabled; 
    }

    private void OnEnable()
    {
        EnableControllers();
        playerInputs.Player.Interact.Enable();
    }

    private void OnDisable()
    {
        DisableControllers();
    }

    private void OnDestroy()
    {
        playerInputs.UI.Pause.started -= context => PauseHandler();
        jumpHandler.OnDestroy(playerInputs);
        movementHandler.OnDestroy(playerInputs);
        rotationHandler.OnDestroy(playerInputs);
        Dialogues.OnDialogue -= SetEnabled;
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
            SetEnabled(true);
            playerRigidbody.useGravity = false;
            playerRigidbody.Sleep();
            GameManager.Instance.UpdateState(GameManager.GameStates.Pause);
        }
        else
        {
            SetEnabled(false);
            playerRigidbody.useGravity = true;
            playerRigidbody.WakeUp();
            GameManager.Instance.UpdateState(GameManager.GameStates.Main);
        }
    }

    /// <summary>
    /// Sets whether the player controller should be working
    /// </summary>
    /// <param name="_isEnabled">Is controller active</param>
    public void SetEnabled(bool _isEnabled)
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
    /// Disables controllers
    /// </summary>
    private void DisableControllers()
    {
        Debug.Log("Disabled player controllers");
        playerInputs.Player.Move.Disable();
        playerInputs.Player.Jumping.Disable();
        playerInputs.Player.Look.Disable();
    }

    /// <summary>
    /// Enables controllers 
    /// </summary>
    private void EnableControllers()
    {
        Debug.Log("Enabled player controllers");
        playerInputs.Player.Move.Enable();
        playerInputs.Player.Jumping.Enable();
        playerInputs.Player.Look.Enable();
    }

    private void OnDrawGizmos()
    {
        jumpHandler.OnGizmos();

        movementHandler.OnGizmos();
    }
}
