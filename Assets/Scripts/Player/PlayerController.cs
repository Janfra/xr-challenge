using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isScenePaused;

    [Header("Dependencies")]
    [SerializeField]
    private Rigidbody playerRigidbody;

    #region Components

    [Header("Components")]
    [SerializeField]
    private PlayerRotation rotationHandler;
    [SerializeField]
    private PlayerJump jumpHandler;
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
    }

    private void Update()
    {
        if(!isScenePaused)
        {
            jumpHandler.GetInputs();
            movementHandler.GetInputs();
            rotationHandler.GetRotation(transform);
        }

        PauseHandler();
    }

    void FixedUpdate()
    {
        if (!isScenePaused)
        {
            jumpHandler.HandleJump();
            movementHandler.HandleMovement();
        }
    }

    private void PauseHandler()
    {
        if (Input.GetKeyDown(KeyCode.Q) && GameManager.Instance.CurrentState != GameManager.GameStates.Pause)
        {
            isScenePaused = true;
            playerRigidbody.useGravity = false;
            playerRigidbody.Sleep();
            GameManager.Instance.UpdateState(GameManager.GameStates.Pause);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            isScenePaused = false;
            playerRigidbody.useGravity = true;
            playerRigidbody.WakeUp();
            GameManager.Instance.UpdateState(GameManager.GameStates.Main);
        }
    }

    private void OnDrawGizmos()
    {
        jumpHandler.OnGizmos();

        movementHandler.OnGizmos();
    }

    /// <summary>
    /// Options explored before arriving at final movement.
    /// </summary>
    #region Option 1
    //  Gets players input and sets the lateral movement and forward movement, then adds them together to set the player movement.
    //private void GetInput()
    //{
    //    float verticalSpeed = speed * Input.GetAxis("Vertical") * Time.deltaTime;
    //    float horizontalSpeed = speed * Input.GetAxis("Horizontal") * Time.deltaTime;

    //    Vector3 lateralMove = horizontalSpeed * transform.right;
    //    Vector3 forwardMove = transform.forward;
    //    forwardMove.y = 0;
    //    forwardMove.Normalize();
    //    forwardMove *= verticalSpeed;

    //    Vector3 move = lateralMove + forwardMove + transform.position;

    //    transform.SetPositionAndRotation(move, Quaternion.identity);
    //}

    #endregion

    #region Option 2

    /// <summary>
    /// Gets the players input and normalizes it to set the omnidirectional movement.
    /// </summary>
    //private void GetPlayersInput()
    //{
    //    float xInput = Input.GetAxisRaw("Horizontal");
    //    float zInput = Input.GetAxisRaw("Vertical");
    //    transform.position += speed * Time.deltaTime * new Vector3(x: xInput, y: 0, z: zInput).normalized;
    //}

    #endregion

    #region Option 3

    //  Tried with only transform, this would have the desired movement, but would not work with collision, and rigidbody would not work as intended

    //  Vector3 newPosition = speed * Time.deltaTime * GetMovementInput() + transform.position;
    //  rigidbodyMove.MovePosition(newPosition);
    //  transform.position += speed * Time.deltaTime * GetMovementInput();

    // Old input getter 
    //private Vector3 GetMovementInput()
    //{
    //    Vector3 forwardMove = speed * Input.GetAxisRaw("Vertical") * transform.forward;
    //    Vector3 lateralMove = speed / SIDEWAYS_PENALTY * Input.GetAxisRaw("Horizontal") * transform.right;
    //    Vector3 resultingMovement = forwardMove + lateralMove;
    //    resultingMovement.Normalize();

    //    return resultingMovement;
    //}

    #endregion
}
