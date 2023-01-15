using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isEnabled;

    [Header("Dependencies")]
    [SerializeField]
    private Rigidbody playerRigidbody;
    private PlayerInputs playerInputs;
     
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
        rotationHandler.Init();
        jumpHandler.Init(playerInputs);

        Dialogues.OnDialogue += SetEnabled; 
    }

    private void OnEnable()
    {
        EnableControllers();
    }

    private void OnDisable()
    {
        DisableControllers();
    }

    private void Update()
    {
        if(!isEnabled)
        {
            jumpHandler.OnUpdate();
            movementHandler.GetInputs();
            rotationHandler.GetRotation(transform);
        }

        PauseHandler();
    }

    private void FixedUpdate()
    {
        if (!isEnabled)
        {
            jumpHandler.HandleJump();
            movementHandler.HandleMovement();
        }
    }

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

    public void SetEnabled(bool _isEnabled)
    {
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

    public void EnableJumping()
    {
        jumpHandler.ResetCoyoteTime();
    }

    private void DisableControllers()
    {
        playerInputs.Player.Move.Disable();
        playerInputs.Player.Jumping.Disable();
    }

    private void EnableControllers()
    {
        playerInputs.Player.Move.Enable();
        playerInputs.Player.Jumping.Enable();
    }

    private void OnDrawGizmos()
    {
        jumpHandler.OnGizmos();

        movementHandler.OnGizmos();
    }
}
