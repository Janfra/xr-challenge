using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerJump
{
    private bool isJumping;

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
        if (IsPlayerOnGround() && !isJumping && Input.GetKeyDown(KeyCode.Space))
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
        isJumping = !isTouchingGround;
        return isTouchingGround;
    }

    /// <summary>
    /// Makes the player jump
    /// </summary>
    private void Jump()
    {
        isJumping = true;
        rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Starts pulling player down
    /// </summary>
    private void StopJumping()
    {
        if(rigidbody.velocity.y > 0)
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
