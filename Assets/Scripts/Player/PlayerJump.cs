using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerJump
{
    private bool isJumping;

    #region Coyote Time

    private float timeSinceGrounded;
    private bool isCoyoteTime => timeSinceGrounded > 0;

    #endregion

    #region Jump Buffer

    private float jumpBufferTime = 0.2f;
    private float timeSinceJump;
    private bool isInJumpTime => timeSinceJump > 0f;

    #endregion

    [Header("Dependencies")]
    [SerializeField]
    private Rigidbody rigidbody;

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
    /// Initializes class
    /// </summary>
    public void Init()
    {
        if(groundCheck == null)
        {
            Debug.LogError("No ground check assigned on player");
        }
    }

    /// <summary>
    /// Clamps on editor changes
    /// </summary>
    public void OnValidate()
    {
        jumpCutMultiplier = Mathf.Clamp01(jumpCutMultiplier);
    }

    /// <summary>
    /// Handles jump logic on update
    /// </summary>
    public void HandleJump()
    {
        isJumping = IsPlayerOnGround();
        UpdateJumpBuffer();

        if (isCoyoteTime && isInJumpTime)
        {
            Jump();
        }

        if (Input.GetKeyUp(KeyCode.Space) && isJumping)
        {
            StopJumping();
        }
    }

    /// <summary>
    /// Checks if the player is colliding with an object marked as jumpable
    /// </summary>
    /// <returns>Is player touching the floor</returns>
    private bool IsPlayerOnGround()
    {
        bool isTouchingGround = Physics.CheckSphere(groundCheck.position, groundCheckSize, jumpableLayer);

        if (isTouchingGround)
        {
            timeSinceGrounded = coyoteTime;
        }
        else
        {
            timeSinceGrounded -= Time.deltaTime;
        }

        return isTouchingGround;
    }

    private void UpdateJumpBuffer()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            timeSinceJump = jumpBufferTime;
        }
        else
        {
            timeSinceJump -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Makes the player jump
    /// </summary>
    private void Jump()
    {
        timeSinceJump = 0;
        rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Starts pulling player down
    /// </summary>
    private void StopJumping()
    {
        if (rigidbody.velocity.y > 0)
        {
            rigidbody.AddForce((1 - jumpCutMultiplier) * rigidbody.velocity.y * Vector3.down, ForceMode.Impulse);
        }
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
