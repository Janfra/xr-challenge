using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMovement 
{
    [Header("Dependencies")]
    [SerializeField]
    private Transform transform;

    #region Config

    [Header("Config")]
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private float wallCheckSize = 0.05f;
    [SerializeField]
    private LayerMask wallLayer;

    #endregion

    #region Variables & Constants

    private Vector3 movementInput;
    private Vector3 movementDirection;

    private const float COLLISION_DECTECTION_DISTANCE_OFFSET = 0.6f;
    private const float SIDEWAYS_PENALTY = 2f;

    #endregion

    public void Init<T>(T _transform) where T : Transform
    {
        transform = _transform;
    }

    public void GetInputs()
    {
        movementInput = GetMovementInput();
        movementDirection = GetRotationOffsetInput();
    }

    public void HandleMovement()
    {
        MovePlayer();
    }

    /// <summary>
    /// Moves the player using the inputs.
    /// </summary>
    private void MovePlayer()
    {
        if (!IsCollidingWithWall())
        {
            transform.Translate(speed * Time.deltaTime * movementInput);
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

    /// <summary>
    /// Returns if the player is going to collide with a wall.
    /// </summary>
    /// <returns>Is the player about to run onto a wall</returns>
    private bool IsCollidingWithWall()
    {
        return Physics.CheckSphere(movementDirection + transform.position, wallCheckSize, wallLayer);
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

    public void OnGizmos()
    {
        Gizmos.DrawSphere(transform.position + GetRotationOffsetInput() + -transform.up, wallCheckSize);
    }
}