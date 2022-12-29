using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isEnabled;

    [Header("Dependencies")]
    [SerializeField]
    private Rigidbody playerRigidbody;

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
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }
    }

    private void Start()
    {
        movementHandler.Init(transform);
        rotationHandler.Init();
        jumpHandler.Init();

        Dialogues.OnDialogue += SetEnabled; 
    }

    private void Update()
    {
        if(!isEnabled)
        {
            jumpHandler.GetInputs();
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
        if (Input.GetKeyDown(KeyCode.Q) && GameManager.Instance.CurrentState != GameManager.GameStates.Pause)
        {
            SetEnabled(true);
            playerRigidbody.useGravity = false;
            playerRigidbody.Sleep();
            GameManager.Instance.UpdateState(GameManager.GameStates.Pause);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
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
    }

    public void SetToGrounded()
    {
        jumpHandler.SetToGrounded();
    }

    public bool IsJumping()
    {
        return jumpHandler.TimeSinceJump == 0;
    }

    private void OnDrawGizmos()
    {
        jumpHandler.OnGizmos();

        movementHandler.OnGizmos();
    }
}
