using UnityEngine;

public class Movement : MonoBehaviour
{
    private float playerYSize;
    [Header("Components")]
    [SerializeField]
    private PlayerRotation rotationHandler;
    [SerializeField]
    private PlayerJump jumpHandler;

    [Header("Config")]
    [SerializeField]
    private float speed;
    [SerializeField]
    private float wallCheckSize;
    [SerializeField]
    private LayerMask wallLayer;

    private const float COLLISION_DECTECTION_DISTANCE_OFFSET = 0.6f;
    private const float SIDEWAYS_PENALTY = 2f;

    private void OnValidate()
    {
        jumpHandler.OnValidate();
    }

    private void Start()
    {
        playerYSize = GetComponent<MeshRenderer>().bounds.size.y;
        rotationHandler.Init();
        jumpHandler.Init();
    }

    private void Update()
    {
        jumpHandler.HandleJump();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    /// <summary>
    /// Moves the player using the inputs.
    /// </summary>
    private void MovePlayer()
    {
        rotationHandler.GetRotation(transform);
        if (!IsCollidingWithWall())
        {
            transform.Translate(speed * Time.deltaTime * GetMovementInput());
        }
    }

    /// <summary>
    /// Get the inputs for movement based on player, add them and normalise them for consistency.
    /// </summary>
    /// <returns>Normalised player movement</returns>
    private Vector3 GetMovementInput()
    {
        Vector3 forwardMove = speed * Input.GetAxisRaw("Vertical") * Vector3.forward;
        Vector3 lateralMove = speed / SIDEWAYS_PENALTY * Input.GetAxisRaw("Horizontal") * Vector3.right;
        Vector3 resultingMovement = forwardMove + lateralMove;
        resultingMovement.Normalize();

        return resultingMovement;
    }

    private bool IsCollidingWithWall()
    {
        return Physics.CheckSphere(GetRotationOffsetInput() + transform.position, wallCheckSize, wallLayer);
    }

    /// <summary>
    /// Get the rotation from the player and modify the input to be in the right position.
    /// </summary>
    /// <returns>Input movement direction</returns>
    private Vector3 GetRotationOffsetInput()
    {
        Vector3 targetPos = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z) * GetMovementInput();
        targetPos *= COLLISION_DECTECTION_DISTANCE_OFFSET;
        return targetPos;
    }

    private void OnDrawGizmos()
    {
        jumpHandler.OnGizmos();

        Gizmos.DrawSphere(transform.position + GetRotationOffsetInput() + -transform.up, wallCheckSize);
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
