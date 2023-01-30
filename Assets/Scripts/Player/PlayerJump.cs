using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerJump
{
    public event Action OnJump;
    public event Action OnGrounded;

    #region Other Variables & Contants

    private bool isPlayerOnGround;

    #region Coyote Time

    private float timeSinceGrounded;
    private bool isCoyoteTime => timeSinceGrounded > 0;

    #endregion

    #region Jump Buffer

    private float timeDelayToJump;
    public float TimeSinceJump { get; private set; }
    private bool isInJumpTime => TimeSinceJump > 0f && timeDelayToJump < 0;

    private const float JUMP_DELAY = 0.3f;

    #endregion

    #region Land Mark

    private const float LAND_MARK_DISTANCE_SHOW = 3f;
    private const float LAND_MARK_MAX_OPACITY = 0.4f;

    #endregion

    private const float MAX_FALLING_SPEED = -12f;

    #endregion

    #region Dependencies

    [Header("Dependencies")]
    [SerializeField]
    private Rigidbody rigidbody;
    [SerializeField]
    private Canvas landMarkCanvas;
    [SerializeField]
    private Image landMarkImage;

    #endregion

    #region Config

    [Header("Config")]
    /// <summary>
    /// Sets the force affecting how high the player jumps
    /// </summary>
    [SerializeField]
    private float jumpForce = 1f;

    /// <summary>
    /// Changes how fast player gets pulled down after it stops jumping
    /// </summary>
    [SerializeField]
    [Range(0f, 1f)]
    private float jumpCutMultiplier = 0.5f;

    /// <summary>
    /// Layers the player can jump on
    /// </summary>
    [SerializeField]
    private LayerMask jumpableLayer;

    /// <summary>
    /// Position to check for the floor collision
    /// </summary>
    [SerializeField]
    private Transform groundCheck;

    /// <summary>
    /// Size of the sphere checking for collision
    /// </summary>
    [SerializeField]
    private float groundCheckSize = 0.05f;

    /// <summary>
    /// Duration where player is allowed to jump after leaving platform
    /// </summary>
    [SerializeField]
    private float coyoteTime = 0.2f;

    /// <summary>
    /// Duration where the player will jump if the floor is hit after pressing jump button
    /// </summary>
    [SerializeField]
    private float jumpBufferTime = 0.2f;

    #endregion

    /// <summary>
    /// Initializes class
    /// </summary>
    public void Init(PlayerController _playerController)
    {
        if(groundCheck == null)
        {
            Debug.LogError("No ground check assigned on player");
        }

        rigidbody = _playerController.PlayerRigidbody;
        _playerController.ActionInputs.Player.Jumping.canceled += context => StopJumping();
        _playerController.ActionInputs.Player.Jumping.started += context => ResetTimeSinceJump();
    }

    /// <summary>
    /// Updates values required for logic to work
    /// </summary>
    public void OnUpdate()
    {
        UpdateJumpBuffer();
        IsPlayerOnGroundUpdate();
        ClampFallingSpeed();
    }
    
    /// <summary>
    /// Clamps the rigidbody velocity when falling
    /// </summary>
    private void ClampFallingSpeed()
    {
        if(rigidbody.velocity.y < MAX_FALLING_SPEED)
        {
            rigidbody.velocity = new Vector3(0, MAX_FALLING_SPEED, 0);
        }
    }

    /// <summary>
    /// Handles jump logic on update
    /// </summary>
    public void HandleJump()
    {
        if (isCoyoteTime && isInJumpTime)
        {
            Jump();
        }
    }

    /// <summary>
    /// Checks if the player is colliding with an object marked as jumpable
    /// </summary>
    /// <returns>Is player touching the floor</returns>
    private void IsPlayerOnGroundUpdate()
    {
        isPlayerOnGround = Physics.CheckSphere(groundCheck.position, groundCheckSize, jumpableLayer);
        if (isPlayerOnGround)
        {
            landMarkCanvas.gameObject.SetActive(false);
            timeSinceGrounded = coyoteTime;
            OnGrounded?.Invoke();

            // Temporary fix for ocassional velocity added when landing. Would have to change it if using rigidbody for movement
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
        }
        else
        {
            // Land mark positioning and showing 
            if(Physics.Raycast(rigidbody.position, Vector3.down, out RaycastHit hit, LAND_MARK_DISTANCE_SHOW, jumpableLayer))
            {
                SetLandMark(hit.point);
            }
            else
            {
                landMarkCanvas.gameObject.SetActive(false);
            }
            timeSinceGrounded -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Updates the jump buffer variables, restarting it or substracting the timer
    /// </summary>
    private void UpdateJumpBuffer()
    {
        TimeSinceJump -= Time.deltaTime;
        timeDelayToJump -= Time.deltaTime;
    }

    /// <summary>
    /// Resets the time since jump to the buffer time
    /// </summary>
    private void ResetTimeSinceJump()
    {
        TimeSinceJump = jumpBufferTime;
    }

    /// <summary>
    /// Makes the player jump
    /// </summary>
    private void Jump()
    {
        JumpSetup();
        rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        OnJump?.Invoke();
    }

    /// <summary>
    /// Restart jump time and delay as well as cancelling Y velocity for consistent jumping.
    /// </summary>
    private void JumpSetup()
    {
        TimeSinceJump = 0;
        timeDelayToJump = JUMP_DELAY;
        rigidbody.velocity = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Starts pulling player down
    /// </summary>
    private void StopJumping()
    {
        if (rigidbody)
        {
            if (rigidbody.velocity.y > 0 && !isPlayerOnGround)
            {
                rigidbody.AddForce((1 - jumpCutMultiplier) * rigidbody.velocity.y * Vector3.down, ForceMode.Impulse);
            }
        }
    }

    /// <summary>
    /// Resets the 'timeSinceGrounded' time to the coyote time, enabling jumping.
    /// </summary>
    public void ResetCoyoteTime()
    {
        timeSinceGrounded = coyoteTime;
    }

    /// <summary>
    /// Sets the landing mark position and colour based on distance.
    /// </summary>
    /// <param name="_position"></param>
    private void SetLandMark(Vector3 _position)
    {
        landMarkCanvas.gameObject.SetActive(true);

        // Set colour based on how far from landing point
        Color imageColour = landMarkImage.color;
        imageColour.a = GetOpacity(LAND_MARK_DISTANCE_SHOW - Vector3.Distance(rigidbody.position, _position));
        landMarkImage.color = imageColour;

        // Offset the position to avoid clipping.
        Vector3 newPos = _position;
        newPos.y += 0.1f;
        landMarkCanvas.transform.position = newPos;
    }

    /// <summary>
    /// Get opacity based on distance given and landing distance show.
    /// </summary>
    /// <param name="_distance">Distance being compared</param>
    /// <returns>Value in between 0 and 1</returns>
    private float GetOpacity(float _distance)
    {
        float minDistance = 1f;
        _distance += minDistance;
        float opacity = Mathf.Clamp((_distance - minDistance) / ((LAND_MARK_DISTANCE_SHOW + minDistance) - minDistance), 0, LAND_MARK_MAX_OPACITY);
        return opacity;
    }

    /// <summary>
    /// Unsubscribes from events on destroy
    /// </summary>
    /// <param name="_inputs">Event owner</param>
    public void OnDestroy(PlayerInputs _inputs)
    {
        _inputs.Player.Jumping.canceled -= context => StopJumping();
        _inputs.Player.Jumping.started -= context => ResetTimeSinceJump();
    }

    /// <summary>
    /// On Gizmos editor draw
    /// </summary>
    public void OnGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, groundCheckSize);
    }
}
