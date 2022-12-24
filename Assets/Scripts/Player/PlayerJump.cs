using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerJump
{
    #region Other Variables & Contants

    private bool isPlayerOnGround;

    #region Coyote Time

    private float timeSinceGrounded;
    private bool isCoyoteTime => timeSinceGrounded > 0;

    #endregion

    #region Jump Buffer

    private float timeDelayToJump;
    private float timeSinceJump;
    private bool isInJumpTime => timeSinceJump > 0f && timeDelayToJump < 0;

    private const float JUMP_DELAY = 0.25f;

    #endregion

    #endregion

    #region Dependencies

    [Header("Dependencies")]
    [SerializeField]
    private Rigidbody rigidbody;

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
    public void Init()
    {
        if(groundCheck == null)
        {
            Debug.LogError("No ground check assigned on player");
        }
    }

    public void GetInputs()
    {
        UpdateJumpBuffer();
        IsPlayerOnGroundUpdate();

        if (Input.GetKeyUp(KeyCode.Space))
        {
            StopJumping();
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
            timeSinceGrounded = coyoteTime;
        }
        else
        {
            timeSinceGrounded -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Updates the jump buffer variables, restarting it or substracting the timer
    /// </summary>
    private void UpdateJumpBuffer()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            timeSinceJump = jumpBufferTime;
        }
        else
        {
            timeSinceJump -= Time.deltaTime;
            timeDelayToJump -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Makes the player jump
    /// </summary>
    private void Jump()
    {
        timeSinceJump = 0;
        timeDelayToJump = JUMP_DELAY;
        rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Starts pulling player down
    /// </summary>
    private void StopJumping()
    {
        if (rigidbody.velocity.y > 0 && !isPlayerOnGround)
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
